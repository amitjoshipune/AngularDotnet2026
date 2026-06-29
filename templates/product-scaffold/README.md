# Product scaffold template

Copy this folder to `products/<your-product-name>/` when starting a new product.

## Suggested steps

```powershell
# From repo root
mkdir products\my-new-product
cd products\my-new-product

# Backend
dotnet new webapi -n MyProduct.Api -f net8.0
cd MyProduct.Api

# Frontend (from products/my-new-product)
npx @angular/cli@17 new frontend --routing --style=scss --ssr=false
```

## Folder checklist

- [ ] `README.md` — what, why, how to run
- [ ] `docs/PITCH.md` — VC 1-pager
- [ ] `docs/DEMO-SCRIPT.md` — 3-minute demo
- [ ] `frontend/` — Angular app
- [ ] `backend/` — .NET 8 API
- [ ] `database/` — SQL migrations
- [ ] `.cursor/rules` — scope Cursor to this product only

See [Engineering Handbook](../docs/ENGINEERING-HANDBOOK.md) for branching and Azure tips.
