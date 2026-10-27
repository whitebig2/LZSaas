using LZ.Model.Enums;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Newtonsoft.Json;

namespace LZ.Model
{
    public class ShoppingCartAmountResult
    {
        /// <summary>
        /// 促销后商品总额
        /// </summary>
        public decimal ProductTotal { get; set; }

        /// <summary>
        /// 优惠券总优惠
        /// </summary>
        public decimal CouponTotal { get; set; }

        /// <summary>
        /// 应付金额
        /// </summary>
        public decimal PayableAmount { get; set; }

        /// <summary>
        /// 每个 SKU 分摊到的优惠金额
        /// </summary>
        public Dictionary<string, decimal> SkuDiscounts { get; set; } = new();

        /// <summary>
        /// 最优组合用到的券
        /// </summary>
        public List<Shop_V_CouponItems> AppliedCoupons { get; set; } = new();
    }

    public class ShoppingCart
    {
        public List<Shop_V_ShoppingCart>? Items { get; set; }

        public List<Shop_V_CouponItems>? Coupons { get; set; }

        public List<Shop_V_CouponProduct>? CouponProducts { get; set; }

        public List<Shop_V_PromotionProduct>? Promotions { get; set; }

        public ShoppingCartAmountResult CartAmountResult { get; set; }
        public List<Shop_V_CouponItems>? UseCoupons { get; set; }

        public decimal GetAmount()
        {
            return CalculateAmount().PayableAmount;
        }

        /// <summary>
        /// 筛选当前购物车可使用的优惠券（有效期内、适用范围匹配、满足门槛且能产生优惠）
        /// </summary>
        public List<Shop_V_CouponItems> GetUsableCoupons()
        {
            var usable = new List<Shop_V_CouponItems>();
            var lines = BuildCartLines();
            if (lines.Count == 0 || Coupons == null || Coupons.Count == 0)
            {
                return usable;
            }

            var remaining = lines.ToDictionary(l => l.Key, l => l.LineAmount, StringComparer.Ordinal);
            foreach (var candidate in BuildCouponCandidates(lines))
            {
                var discount = CalculateCouponDiscount(candidate, remaining);
                if (discount.Values.Sum() > 0)
                {
                    usable.Add(candidate.Coupon);
                }
            }

            return usable;
        }

        public ShoppingCartAmountResult CalculateAmount()
        {
            var result = new ShoppingCartAmountResult();
            var lines = BuildCartLines();
            if (lines.Count == 0)
            {
                return result;
            }
            result.ProductTotal = RoundMoney(lines.Sum(l => l.LineAmount));

            var candidates = BuildCouponCandidates(lines);
            var best = SelectBestCouponCombination(lines, candidates);

            result.CouponTotal = best.CouponTotal;
            result.AppliedCoupons = best.AppliedCoupons;
            result.SkuDiscounts = best.SkuDiscounts;
            result.PayableAmount = RoundMoney(Math.Max(0, result.ProductTotal - result.CouponTotal));
            return result;
        }

        #region Cart lines / promotion

        private sealed class CartLine
        {
            public string Key { get; set; } = string.Empty;
            public string SkuID { get; set; } = string.Empty;
            public int? ProductID { get; set; }
            public int? ShopID { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal LineAmount { get; set; }
        }

        private List<CartLine> BuildCartLines()
        {
            var lines = new List<CartLine>();
            if (Items == null || Items.Count == 0)
            {
                return lines;
            }

            var index = 0;
            foreach (var item in Items)
            {
                var qty = item.ProductCount ?? 0;
                if (qty <= 0)
                {
                    continue;
                }

                var unitPrice = GetPromotionUnitPrice(item);
                item.PromotionPrice = unitPrice;
                var skuId = item.SkuID ?? string.Empty;
                lines.Add(new CartLine
                {
                    Key = $"{skuId}#{index}",
                    SkuID = skuId,
                    ProductID = item.ProductID,
                    ShopID = item.ShopID,
                    Quantity = qty,
                    UnitPrice = unitPrice,
                    LineAmount = RoundMoney(unitPrice * qty)
                });
                index++;
            }
            return lines;
        }

        private decimal GetPromotionUnitPrice(Shop_V_ShoppingCart item)
        {
            var salePrice = item.SalePrice;
            if (Promotions == null || Promotions.Count == 0 || string.IsNullOrEmpty(item.SkuID))
            {
                return salePrice;
            }

            var promotion = Promotions.Find(p =>
                !string.IsNullOrEmpty(p.SkuID) &&
                string.Equals(p.SkuID, item.SkuID, StringComparison.OrdinalIgnoreCase) &&
                p.Enable &&
                p.PromotionEnable &&
                IsInTimeRange(p.StartTime, p.EndTime));

            if (promotion == null)
            {
                return salePrice;
            }

            if (promotion.PromotionType == (int)PromotionType.Discount &&
                promotion.PromotionDiscount.HasValue &&
                promotion.PromotionDiscount.Value > 0)
            {
                return RoundMoney(salePrice * promotion.PromotionDiscount.Value);
            }

            if (promotion.PromotionType == (int)PromotionType.Promotion &&
                promotion.PromotionMoney > 0)
            {
                return RoundMoney(promotion.PromotionMoney);
            }

            return salePrice;
        }

        #endregion

        #region Coupon eligibility & discount

        private sealed class CouponCandidate
        {
            public Shop_V_CouponItems Coupon { get; set; } = null!;
            public CouponType Type { get; set; }
            public bool IsStack { get; set; }
            public List<CartLine> EligibleLines { get; set; } = new();
        }

        private sealed class CombinationResult
        {
            public decimal CouponTotal { get; set; }
            public List<Shop_V_CouponItems> AppliedCoupons { get; set; } = new();
            public Dictionary<string, decimal> SkuDiscounts { get; set; } = new();
        }

        private List<CouponCandidate> BuildCouponCandidates(List<CartLine> lines)
        {
            var candidates = new List<CouponCandidate>();
            if (Coupons == null || Coupons.Count == 0)
            {
                return candidates;
            }

            foreach (var coupon in Coupons)
            {
                if (!IsCouponValid(coupon))
                {
                    continue;
                }

                var type = ResolveCouponType(coupon);
                if (type == null)
                {
                    continue;
                }

                var eligible = GetEligibleLines(coupon, lines);
                if (eligible.Count == 0)
                {
                    continue;
                }

                candidates.Add(new CouponCandidate
                {
                    Coupon = coupon,
                    Type = type.Value,
                    IsStack = coupon.IsStack,
                    EligibleLines = eligible
                });
            }

            return candidates;
        }

        private static bool IsCouponValid(Shop_V_CouponItems coupon)
        {
            if (!IsInTimeRange(coupon.StartTime, coupon.EndTime))
            {
                return false;
            }

            // 已使用/作废的券不参与计算（若状态有值）
            if (coupon.Status.HasValue &&
                (coupon.Status == (int)CouponItemStatus.HaveUse || coupon.Status == (int)CouponItemStatus.Void))
            {
                return false;
            }

            // 购物车金额计算仅处理商品通用券、指定商品券
            var useType = ResolveCouponUseType(coupon);
            if (useType != CouponUseType.ProductCurrency && useType != CouponUseType.AssignProduct)
            {
                return false;
            }

            return true;
        }

        private static CouponType? ResolveCouponType(Shop_V_CouponItems coupon)
        {
            var typeValue = coupon.MainCouponType ?? coupon.CouponType;
            if (!typeValue.HasValue || !Enum.IsDefined(typeof(CouponType), typeValue.Value))
            {
                return null;
            }

            return (CouponType)typeValue.Value;
        }

        private static CouponUseType? ResolveCouponUseType(Shop_V_CouponItems coupon)
        {
            if (!coupon.CouponUseType.HasValue ||
                !Enum.IsDefined(typeof(CouponUseType), coupon.CouponUseType.Value))
            {
                return null;
            }

            return (CouponUseType)coupon.CouponUseType.Value;
        }

        private List<CartLine> GetEligibleLines(Shop_V_CouponItems coupon, List<CartLine> lines)
        {
            var useType = ResolveCouponUseType(coupon);

            // 商品通用券：适用店铺内全部商品（无 ShopID 则全部行）
            if (useType == CouponUseType.ProductCurrency)
            {
                if (coupon.ShopID.HasValue && coupon.ShopID.Value > 0)
                {
                    return lines.Where(line => line.ShopID == coupon.ShopID).ToList();
                }

                return lines.ToList();
            }

            // 指定商品券：仅 CouponProducts / 券上商品字段匹配的行
            if (useType == CouponUseType.AssignProduct)
            {
                return GetAssignProductEligibleLines(coupon, lines);
            }

            return new List<CartLine>();
        }

        private List<CartLine> GetAssignProductEligibleLines(Shop_V_CouponItems coupon, List<CartLine> lines)
        {
            var couponProducts = GetCouponProducts(coupon);
            if (couponProducts.Count > 0)
            {
                return lines.Where(line => couponProducts.Any(cp => MatchCouponProduct(cp, line))).ToList();
            }

            var productId = coupon.MainProductID ?? coupon.ProductID;
            if (productId.HasValue)
            {
                return lines.Where(line =>
                {
                    return productId.HasValue && line.ProductID == productId;
                }).ToList();
            }

            return new List<CartLine>();
        }

        private List<Shop_V_CouponProduct> GetCouponProducts(Shop_V_CouponItems coupon)
        {
            if (CouponProducts == null || CouponProducts.Count == 0)
            {
                return new List<Shop_V_CouponProduct>();
            }

            return CouponProducts.Where(cp =>
            {
                if (coupon.CouponID.HasValue && cp.CouponID.HasValue && cp.CouponID == coupon.CouponID)
                {
                    return true;
                }

                if (coupon.CouponGID.HasValue && cp.CouponGID.HasValue && cp.CouponGID == coupon.CouponGID)
                {
                    return true;
                }

                return false;
            }).ToList();
        }

        private static bool MatchCouponProduct(Shop_V_CouponProduct cp, CartLine line)
        {
            if (!string.IsNullOrEmpty(cp.SkuID))
            {
                return string.Equals(line.SkuID, cp.SkuID, StringComparison.OrdinalIgnoreCase);
            }

            return cp.ProductID.HasValue && line.ProductID == cp.ProductID;
        }

        /// <summary>
        /// 在给定剩余可优惠金额上计算单券优惠，并返回按行 Key 的分摊。
        /// </summary>
        private static Dictionary<string, decimal> CalculateCouponDiscount(
            CouponCandidate candidate,
            Dictionary<string, decimal> remainingByKey)
        {
            var discounts = new Dictionary<string, decimal>(StringComparer.Ordinal);
            var eligibleRemaining = candidate.EligibleLines
                .Select(l => new
                {
                    Line = l,
                    Remaining = remainingByKey.TryGetValue(l.Key, out var r) ? r : 0M
                })
                .Where(x => x.Remaining > 0)
                .ToList();

            if (eligibleRemaining.Count == 0)
            {
                return discounts;
            }

            var eligibleAmount = eligibleRemaining.Sum(x => x.Remaining);
            var limitMoney = candidate.Coupon.LimitMoney;
            if (limitMoney > 0 && eligibleAmount < limitMoney)
            {
                return discounts;
            }

            switch (candidate.Type)
            {
                case CouponType.Deduct:
                case CouponType.Voucher:
                    {
                        var couponMoney = ResolveCouponMoney(candidate.Coupon);
                        if (couponMoney <= 0)
                        {
                            return discounts;
                        }

                        var discount = Math.Min(couponMoney, eligibleAmount);
                        return AllocateByRatio(eligibleRemaining.Select(x => (x.Line.Key, x.Remaining)).ToList(), discount);
                    }
                case CouponType.Discount:
                    {
                        var discountRate = ResolveCouponDiscount(candidate.Coupon);
                        if (discountRate <= 0 || discountRate >= 1)
                        {
                            return discounts;
                        }

                        foreach (var item in eligibleRemaining)
                        {
                            var lineDiscount = RoundMoney(item.Remaining * (1 - discountRate));
                            if (lineDiscount > 0)
                            {
                                discounts[item.Line.Key] = lineDiscount;
                            }
                        }

                        return discounts;
                    }
                case CouponType.Experience:
                    {
                        var freeCount = ResolveProductCount(candidate.Coupon);
                        if (freeCount <= 0)
                        {
                            return discounts;
                        }

                        // 按单价从高到低免件
                        var ordered = eligibleRemaining
                            .OrderByDescending(x => x.Line.UnitPrice)
                            .ThenByDescending(x => x.Remaining)
                            .ToList();

                        var left = freeCount;
                        foreach (var item in ordered)
                        {
                            if (left <= 0)
                            {
                                break;
                            }

                            var freeQty = Math.Min(left, item.Line.Quantity);
                            var freeAmount = RoundMoney(item.Line.UnitPrice * freeQty);
                            freeAmount = Math.Min(freeAmount, item.Remaining);
                            if (freeAmount > 0)
                            {
                                discounts[item.Line.Key] = freeAmount;
                                left -= freeQty;
                            }
                        }

                        return discounts;
                    }
                default:
                    return discounts;
            }
        }

        private static decimal ResolveCouponMoney(Shop_V_CouponItems coupon)
        {
            return coupon.MainCouponMoney > 0 ? coupon.MainCouponMoney : coupon.CouponMoney;
        }

        private static decimal ResolveCouponDiscount(Shop_V_CouponItems coupon)
        {
            if (coupon.MainCouponDiscount.HasValue && coupon.MainCouponDiscount.Value > 0)
            {
                return coupon.MainCouponDiscount.Value;
            }

            return coupon.CouponDiscount ?? 0M;
        }

        private static int ResolveProductCount(Shop_V_CouponItems coupon)
        {
            return coupon.MainProductCount ?? coupon.ProductCount ?? 0;
        }

        #endregion

        #region Optimal combination / stack

        private CombinationResult SelectBestCouponCombination(List<CartLine> lines, List<CouponCandidate> candidates)
        {
            var empty = new CombinationResult
            {
                SkuDiscounts = lines
                    .GroupBy(l => l.SkuID)
                    .ToDictionary(g => g.Key, _ => 0M, StringComparer.OrdinalIgnoreCase)
            };

            if (candidates.Count == 0)
            {
                return empty;
            }

            var combinations = new List<List<CouponCandidate>>();
            var stackable = candidates.Where(c => c.IsStack).ToList();
            var exclusive = candidates.Where(c => !c.IsStack).ToList();

            if (stackable.Count > 0)
            {
                combinations.Add(stackable);
            }

            foreach (var coupon in exclusive)
            {
                combinations.Add(new List<CouponCandidate> { coupon });
            }

            // 无叠加券时，仍评估每张可叠加券单独使用（若只有一张已在上面覆盖）
            if (combinations.Count == 0)
            {
                return empty;
            }

            CombinationResult? best = null;
            foreach (var combo in combinations)
            {
                var evaluated = EvaluateCombination(lines, combo);
                if (best == null ||
                    evaluated.CouponTotal > best.CouponTotal ||
                    (evaluated.CouponTotal == best.CouponTotal &&
                     evaluated.CouponTotal > 0 &&
                     evaluated.AppliedCoupons.Count < best.AppliedCoupons.Count))
                {
                    best = evaluated;
                }
            }

            return best ?? empty;
        }

        private CombinationResult EvaluateCombination(List<CartLine> lines, List<CouponCandidate> combo)
        {
            var remaining = lines.ToDictionary(l => l.Key, l => l.LineAmount, StringComparer.Ordinal);
            var lineDiscounts = lines.ToDictionary(l => l.Key, _ => 0M, StringComparer.Ordinal);
            var applied = new List<Shop_V_CouponItems>();

            // 固定金额券先于折扣券，体验券最后（更利于叠加后的总优惠）
            var ordered = combo
                .OrderBy(c => GetCouponApplyOrder(c.Type))
                .ThenByDescending(c => EstimateCouponPriority(c, remaining))
                .ToList();

            foreach (var candidate in ordered)
            {
                var perLine = CalculateCouponDiscount(candidate, remaining);
                var total = perLine.Values.Sum();
                if (total <= 0)
                {
                    continue;
                }

                foreach (var kv in perLine)
                {
                    lineDiscounts[kv.Key] = RoundMoney(lineDiscounts[kv.Key] + kv.Value);
                    remaining[kv.Key] = RoundMoney(Math.Max(0, remaining[kv.Key] - kv.Value));
                }

                applied.Add(candidate.Coupon);
            }

            var skuDiscounts = AggregateSkuDiscounts(lines, lineDiscounts);
            var couponTotal = RoundMoney(skuDiscounts.Values.Sum());
            FixSkuDiscountTail(skuDiscounts, couponTotal);

            return new CombinationResult
            {
                CouponTotal = couponTotal,
                AppliedCoupons = applied,
                SkuDiscounts = skuDiscounts
            };
        }

        private static int GetCouponApplyOrder(CouponType type)
        {
            return type switch
            {
                CouponType.Experience => 0,
                CouponType.Deduct => 1,
                CouponType.Voucher => 1,
                CouponType.Discount => 2,
                _ => 9
            };
        }

        private static decimal EstimateCouponPriority(CouponCandidate candidate, Dictionary<string, decimal> remaining)
        {
            var eligibleAmount = candidate.EligibleLines.Sum(l =>
                remaining.TryGetValue(l.Key, out var r) ? r : 0M);

            switch (candidate.Type)
            {
                case CouponType.Deduct:
                case CouponType.Voucher:
                    return Math.Min(ResolveCouponMoney(candidate.Coupon), eligibleAmount);
                case CouponType.Discount:
                    {
                        var rate = ResolveCouponDiscount(candidate.Coupon);
                        return rate > 0 && rate < 1 ? eligibleAmount * (1 - rate) : 0M;
                    }
                case CouponType.Experience:
                    {
                        var freeCount = ResolveProductCount(candidate.Coupon);
                        return candidate.EligibleLines
                            .OrderByDescending(l => l.UnitPrice)
                            .Take(Math.Max(freeCount, 0))
                            .Sum(l => l.UnitPrice);
                    }
                default:
                    return 0M;
            }
        }

        #endregion

        #region Allocation helpers

        private static Dictionary<string, decimal> AllocateByRatio(
            List<(string Key, decimal Amount)> parts,
            decimal discount)
        {
            var result = new Dictionary<string, decimal>(StringComparer.Ordinal);
            if (discount <= 0 || parts.Count == 0)
            {
                return result;
            }

            var total = parts.Sum(p => p.Amount);
            if (total <= 0)
            {
                return result;
            }

            discount = Math.Min(RoundMoney(discount), RoundMoney(total));
            var allocated = 0M;
            for (var i = 0; i < parts.Count; i++)
            {
                var part = parts[i];
                decimal share;
                if (i == parts.Count - 1)
                {
                    share = RoundMoney(discount - allocated);
                }
                else
                {
                    share = RoundMoney(discount * (part.Amount / total));
                    allocated += share;
                }

                share = Math.Min(share, part.Amount);
                if (share > 0)
                {
                    result[part.Key] = share;
                }
            }

            // 尾差修正到金额最大行
            var sum = result.Values.Sum();
            var diff = RoundMoney(discount - sum);
            if (diff != 0 && result.Count > 0)
            {
                var maxKey = parts.OrderByDescending(p => p.Amount).First().Key;
                if (result.ContainsKey(maxKey))
                {
                    result[maxKey] = RoundMoney(Math.Max(0, result[maxKey] + diff));
                }
                else
                {
                    result[maxKey] = RoundMoney(Math.Max(0, diff));
                }
            }

            return result;
        }

        private static Dictionary<string, decimal> AggregateSkuDiscounts(
            List<CartLine> lines,
            Dictionary<string, decimal> lineDiscounts)
        {
            var skuDiscounts = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            foreach (var line in lines)
            {
                var discount = lineDiscounts.TryGetValue(line.Key, out var d) ? d : 0M;
                if (!skuDiscounts.ContainsKey(line.SkuID))
                {
                    skuDiscounts[line.SkuID] = 0M;
                }

                skuDiscounts[line.SkuID] = RoundMoney(skuDiscounts[line.SkuID] + discount);
            }

            return skuDiscounts;
        }

        private static void FixSkuDiscountTail(Dictionary<string, decimal> skuDiscounts, decimal expectedTotal)
        {
            if (skuDiscounts.Count == 0)
            {
                return;
            }

            var sum = RoundMoney(skuDiscounts.Values.Sum());
            var diff = RoundMoney(expectedTotal - sum);
            if (diff == 0)
            {
                return;
            }

            var maxKey = skuDiscounts.OrderByDescending(kv => kv.Value).First().Key;
            skuDiscounts[maxKey] = RoundMoney(Math.Max(0, skuDiscounts[maxKey] + diff));
        }

        private static bool IsInTimeRange(DateTime? start, DateTime? end)
        {
            var now = DateTime.Now;
            if (start.HasValue && now < start.Value)
            {
                return false;
            }

            if (end.HasValue && now > end.Value)
            {
                return false;
            }

            return true;
        }

        private static decimal RoundMoney(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        #endregion
    }
}
