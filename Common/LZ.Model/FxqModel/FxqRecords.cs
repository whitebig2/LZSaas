using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.FxqModel
{


    #region 获取Token
    /// <summary>
    /// 获取Token
    /// </summary>
    /// <param name="Key">AppID</param>
    /// <param name="Secret">AppSecret</param>
    public record TokenRequest(string Key, string Secret);
    #endregion

    #region 个人
    /// <summary>
    /// 个人公安二要素
    /// </summary>
    /// <param name="RealName">姓名</param>
    /// <param name="IdCardNo">身份证号码</param>
    public record RealName2Request(string RealName, string IdCardNo);
    /// <summary>
    /// 全网三要素认证
    /// </summary>
    /// <param name="RealName">姓名</param>
    /// <param name="IdCardNo">身份证号码</param>
    /// <param name="Mobile">手机号码</param>
    public record RealName3Request(string RealName, string IdCardNo, string Mobile);
    /// <summary>
    /// 银行卡三要素
    /// </summary>
    /// <param name="Name">姓名</param>
    /// <param name="IdCardNo">身份证号码</param>
    /// <param name="BankCardNo">银行卡号</param>
    public record RealNameBank3Request(string Name, string IdCardNo, string BankCardNo);
    public record RealNameBank4Request(string Name, string IdCardNo, string BankCardNo, string Mobile);
    /// <summary>
    /// 个人实名全⽹三要素+短信 (直连CA) 新的认证方式
    /// </summary>
    /// <param name="PersonName">真实姓名</param>
    /// <param name="IdNo">证件号码号码</param>
    /// <param name="Phone">手机号码</param>
    /// /// <param name="IdType">0、身份证号</param>
    /// /// <param name="RedirectUrl">签发证书成功后跳转地址</param>
    public record RealNameMessageRequest(string PersonName, string IdNo, string Phone, string IdType, string RedirectUrl);

    /// <summary>
    /// 个人认证响应
    /// </summary>
    /// <param name="State">
    /// 1：实名认证结果匹配
    ///2：实名认证结果不匹配
    ///3：其他异常情况
    /// </param>
    public record RealNameStatus(int State);
    public record CaRealNameStatus(int State,string errMsg);
    /// <summary>
    /// 实名认证证书申请请求
    /// </summary>
    /// </param>

    public record RealNameApply(bool Success, string Msg, string ApplyId, string AgreementUrl);

    #endregion
    #region 公司


    /// <summary>
    /// 企业四要素认证
    /// </summary>
    /// <param name="Name">公司名称</param>
    /// <param name="Keyword">注册号/统一社会信用代码(注册号支持15位，统一社会信用代码支持18位)</param>
    /// <param name="LegalPerson">企业法人</param>
    public record CompanyRealName3Request(string Name, string Keyword, string LegalPerson);
    /// <summary>
    /// 企业四要素认证
    /// </summary>
    /// <param name="Name">公司名称</param>
    /// <param name="Keyword">注册号/统一社会信用代码(注册号支持15位，统一社会信用代码支持18位)</param>
    /// <param name="LegalPerson">企业法人</param>
    /// <param name="CertNo">企业法人身份证号</param>
    public record CompanyRealName4Request(string Name, string Keyword, string LegalPerson, string CertNo);

    /// <summary>
    /// 企业工商照面信息查询
    /// </summary>
    /// <param name="Name">企业名称</param>
    public record CompanyNameRequest(string Name);
    public record CompanyBankPaymentRequest(string AccountName,string BankName,string No);




    /// <summary>
    /// 企业三要素响应
    /// </summary>
    /// <param name="State">验证结果(1:全匹配,2:全不匹配,12:公司匹配&&法人不匹配,13:公司不匹配&&法人匹配）</param>
    /// <param name="LegalName">法人姓名</param>
    /// <param name="Name">公司名称</param>
    public record CompanyRealName3Response(int Status, string LegalName, string Name);
    /// <summary>
    /// 企业四要素响应
    /// </summary>
    /// <param name="entNameMatch">公司名称匹配结果 (1:匹配 2:不匹配)</param>
    /// <param name="entRegNoMatch">工商注册号匹配结果 (1:匹配 2:不匹配)</param>
    /// <param name="repNameMatch">法人姓名匹配结果 (1:匹配 2:不匹配)</param>
    /// <param name="repIdNoMatch">法人身份证匹配结果 (1:匹配 2:不匹配)</param>
    public record CompanyRealName4Response(int entNameMatch, int entRegNoMatch, int repNameMatch, int repIdNoMatch);
    public record CaCompanyRealName4Response(int entNameMatch, int entRegNoMatch, int repNameMatch, int repIdNoMatch,bool match);
    public record CompanyNameData(string RegistCapi, string Status, string CreditCode, string No, string BelongOrg, string OperName, string EconKind, string Address, string UpdatedDate, int EntType, string RecCap, string OrgNo, string[] OriginalName, string EndDate, string Province, string TermStart, string Name, string StartDate, string TeamEnd, string Scope, string StockType, int IsOnStock, string StockNumber, string CheckDate);
    public record CompanyNameResponse(string result, int entRegNoMatch, string resultMsg, CompanyNameData data);

    public record CompanyBankPaymentResponse(string amount, string verify_code);

    /// <summary>
    /// 企业实名全⽹三要素+短信 (直连CA) 新的认证方式
    /// </summary>
    /// <param name="LegalName">法人姓名</param>
    /// <param name="LegalIdNo">法人身份证号</param>
    /// <param name="CompanyName">企业名称</param>
    /// /// <param name="CreditNo">企业信用代码</param>
    /// /// <param name="RedirectUrl">签发证书成功后跳转地址</param>
    public record CompanyMessageRequest(string LegalName, string LegalIdNo, string CompanyName, string CreditNo, string RedirectUrl);

    #endregion
    #region 意愿认证
    /// <summary>
    /// 短信
    /// </summary>
    /// <param name="Phone">手机号码</param>
    /// <param name="Content">消息内容</param>
    public record SmsSendRequest(string Phone,string Content);
    /// <summary>
    /// 消息响应
    /// </summary>
    /// <param name="MsgId">消息Id</param>
    /// <param name="Time">发送时间</param>
    public record SmsSendResponse(string MsgId, string Time);

    #endregion
    #region 合同签署
    /// <summary>
    ///  签章位置
    /// </summary>
    /// <param name="X">区域左下角x坐标</param>
    /// <param name="Y">区域左下角y坐标</param>
    /// <param name="Page">合同页数(第一页为1)</param>
    public record SignArea(float X, float Y,int Page);
    /// <summary>
    /// 签署人/签署单位 信息
    /// </summary>
    /// <param name="Name">签署人名称 / 企业名称</param>
    /// <param name="Idno">身份证号码 / 社会统一信用代码</param>
    /// <param name="Seal">签章图片(Base64或oss的URL链接；仅支持png格式，建议图片背景为透明无底) 注意：传bases64时，不能传头信息</param>
    /// <param name="areas">签署人的签署区域(可以同时传多个区域)</param>
    /// <param name="Size">签章大小（局部），默认 120(单位为px)</param>
    /// <param name="Height">传此参数加盖骑缝章，不传则不加盖骑缝章，传值控制骑缝章在文件中的上下位置(建议值在100 - 600之间)</param>
    public record Signer(string Name, string Idno, string Seal, SignArea[] Areas,int Size,int Height);
    /// <summary>
    /// 签章日期
    /// </summary>
    /// <param name="Enable">值为1，若不需要显示签章日期，则不传 signDate参数即可</param>
    /// <param name="ColorType">签章日期字体颜色(1:黑色,2:红色)</param>
    /// <param name="MoveX">移动控制签章日期X轴运动（值为正数沿X轴向左运动，负数沿X轴向右运动）</param>
    /// <param name="MoveY">移动控制签章日期Y轴运动（值为正数沿Y轴向下运动，负数沿Y轴向上运动）</param>
    public record SignDate(int Enable, int? ColorType, int? MoveX,  int? MoveY);


    /// <summary>
    /// pdf 单文件签署
    /// </summary>
    /// <param name="Contract">合同文件(Base64或oss的URL链接)注意：传bases64时，不能传头信息</param>
    /// <param name="Signers">签署人信息(具体信息,可以同时传多个签署人)</param>
    /// <param name="Type">合同返回类型(0:Base64,1:oss地址,默认为0)</param>
    /// <param name="Size">签章的大小(全局)，默认 120(单位为px)</param>
    /// <param name="SignDate">签章日期（默认跟随在印章底部）
    /// 
    /// 
    /// </param>
    public record PdfContractSignRequest(string Contract, Signer[] Signers,int Type,int Size, SignDate SignDate);


    public record KeywordSigner(string Name, string Idno, string Seal, string Key, int? Size, int? Height);
    public record KeywordPdfContractSignRequest(string Contract, KeywordSigner[] Signers, int Type,int? Move, int? Size, SignDate? SignDate);
    #endregion

    #region 合同存证

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Content">文本存证时，表示所存的原文信息（注意：输入值根据实际填写）,文件存证时，表示源文件网络地址或者base64值</param>
    /// <param name="EvidenceType">存证类型，默认值为1：
    ///1:文本存证：如实名、意愿等流程证据存证，大小内容需小于100K。
    ///2:文件存证：如合同原文存证。</param>
    /// <param name="TransactionId">存证事务id，标识一组关联的存证链路
    ///1.首次存证不需要传(自动创建)
    ///2.关联的一组存证链路请使用同一个transactionId</param>
    /// <param name="Phase">当前所处存证事务中的阶段，请合作方自行定义-文本存证时必传，注意：文件存证时不须要传，阶段默认就是合同签署</param>
    public record EvidenceFullRequest(string Content, string EvidenceType, string TransactionId, string Phase);
    public record EvidenceFullResponse(string TransactionId, string Hash);

    public record SignAndEvidenceFullResponse(string PdfBase64, string TransactionId, string Hash);

    /// <summary>
    /// 存证返回的hash
    /// </summary>
    /// <param name="Dna"></param>
    public record EvidenceLetterRequest(string Dna);
    public record EvidenceLetterResponse(string Pdf,string RequestId);


    public record EvidenceFullFetchRequest(string EvidenceType,string TxHash);

    #endregion

    #region 辅助接口

    /// <summary>
    /// 待替换的文档(Base64,不带头信息)
    /// </summary>
    /// <param name="Base64"></param>
    public record WordToPdfRequest(string Base64);
    /// <summary>
    /// 个人签章生成
    /// </summary>
    /// <param name="Name">签署人名称（不能超过15个字符）</param>
    /// <param name="rtype">返回类型(0:Base64,1:oss地址链接,默认为0)</param>
    /// <param name="color">颜色(0:红色,1:蓝色,2:黑色,默认为0)</param>
    /// <param name="font">字体(0:仿宋,1:楷体,2:黑体,默认为0)</param>
    /// <param name="type">形状(0:长方形,1:正方形,默认为0)</param>
    public record ElectronicSealRequest(string Name,int? Rtype,int? Color,int? Font,int? Type);
    /// <summary>
    /// 企业签章生成
    /// </summary>
    /// <param name="Name">签署公司名称</param>
    /// <param name="Title">副标题</param>
    /// <param name="SeqNo">下弦文</param>
    /// <param name="Rtype">返回类型(0:Base64,1:oss的URL链接,默认为0)</param>
    /// <param name="Color">颜色(0:红色,1:蓝色,2:黑色,默认为0)</param>
    /// <param name="Font">字体(0:仿宋,1:楷体,2:黑体,默认为0)</param>
    /// <param name="Type">签章类型(0:圆型,1:椭圆,默认为0)</param>
    /// <param name="isRound">签章样式（0:签章做旧，1:默认样式，默认为0）</param>
    public record CompanyElectronicSealRequest(string Name, string? Title, string? SeqNo, int? Rtype, int? Color, int? Font, int? Type, int? isRound);

    public record CheckFileSignatureRequest(string FileUrl);
    /// <summary>
    /// pdf转Png
    /// </summary>
    /// <param name="FileUrl"></param>
    public record PdfToPngRequest(string FileUrl);

    public record PngItem(int Page,string Png);
    public record PdfToPngResponse(string TaskId,int Count, PngItem[] Pngs);

    /// <summary>
    /// 合同url地址（签署接口返回的oss链接）
    /// </summary>
    /// <param name="contractUrl"></param>
    public record ContractRemoveRequest(string ContractUrl);
    public record BankInfoRequest(string BankCardNo);
    public record BankInfoResponse(string Bank,string BankCode,string Province,string City,string CardName,string CardBin,string Type);
    /// <summary>
    /// 关键词替换
    /// </summary>
    /// <param name="Contract">合同文件(Base64/oss的URL链接)</param>
    /// <param name="Replaces">关键词替换内容，具体参考入参json</param>
    /// <param name="Rtype">返回类型(0:Base64,1:oss的URL链接,默认为0)</param>
    public record DocxKeywordReplaceRequest<T>(string Contract,T Replaces, int Rtype);
    #endregion

    public record CaFaceFileResponse(string oriCode, string orderNo, string liveRate, string similarity, string occurredTime, bool success, string photo, string video,string bizSeqNo,string transactionTime);
}
