# Reimbursement: Codex Instructions

These instructions cover reimbursement and approval work across this repository, including shared `Services/LZ.Finance.Service` and `Common/LZ.Model` files named in the root `AGENTS.md`.

## Ownership and references

- API controllers: `Apis/LZ.Reimbursement.API/Controllers`; business interfaces and services: `Services/LZ.Finance.Service/Interface` and `Services/LZ.Finance.Service/Services`; DTOs: `Common/LZ.Model/Dtos/Reimbursement` and `Dtos/Reimbusement`; enums: `Common/LZ.Model/Enums/Reimbusement` (the directory spelling is intentional in current source); finance entities: `Common/LZ.Model/Model/T4/FinanceEntity.cs`.
- The reimbursement and budget documents under `D:\业务文档\Reimbursement` are useful references when available. Source code and actual schema take precedence when documents are stale.
- `ReimbursementBudget*` controllers and services are copied budget-management code; confirm whether each is active before using it as evidence for the reimbursement application path.
- The existing budget-management implementation in `Apis/LZ.Finance.API/Controllers/Budget*` and `Services/LZ.Finance.Service/Services/Budget*` is a production dependency. Do not edit its code or configuration for reimbursement work. Add adaptations on the reimbursement side.

## Business-chain checks

- Trace a change through submit/resubmit/copy, invoice recognition and verification, deduplication, approval, payment, supplement, read models, and failure/idempotency paths as applicable. Preserve company isolation, actor authorization, status transitions, amount precision, and transaction/compensation behavior.
- Reimbursement identity uses `Live_Personnel`. Do not infer an ID relationship with `SYS_CompanyUser`. Verify user and department IDs at each entry point; keep approval-action authorization tied to the assigned node. The temporary MiniAdmin full-company list visibility is a query-only accommodation with a TODO in code; do not extend it to approval actions.
- For budget-flow changes, verify whether the source is `Live_BudgetManage` (Main) or `FIN_Reimb_BudgetManage` (Finance). They are separate pools. Match on company, year, partner, cost type, and expense title as required by the actual legacy pool. Distinguish a read-only amount comparison from reserve/commit/release accounting; never silently treat one as the other.
- For invoice changes, reuse existing `IHttpService` and established OCR/verification integrations. Validate per-invoice identity and duplicates before summing valid amounts. A payment application's supplement status must reflect cumulative valid invoice amount against its actual apply amount.

## Change boundary and verification

- Existing reimbursement source may be changed when needed for a complete requested flow. Keep shared DTO, enum, or entity edits limited to reimbursement data contracts. Do not alter unrelated modules or global configuration without explicit user authorization.
- Preserve existing local edits. Use focused builds for affected projects; leave unit and integration tests to the user unless requested. State clearly when database, upstream services, or runtime behavior could not be verified.
