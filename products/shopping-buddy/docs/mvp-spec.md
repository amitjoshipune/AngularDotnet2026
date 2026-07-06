# MVP specification — Week 1 foundation

Last updated: 2026-07-06

## Decisions locked for MVP

| Topic | Decision |
|-------|----------|
| Deploy | Local demo + GitHub (Weeks 1–3); free static Angular host Week 4 |
| Azure | Opt-in only; no paid resources by default |
| Database | SQL Server scripts + Dapper (no EF Core) |
| Roles | **Customer**, **Buddy**, **Admin** |
| Legacy test-app UI | Hidden from routing and navbar (components kept in repo) |

## Booking fields

| Field | Type | Notes |
|-------|------|-------|
| Buddy | required | From search results |
| Venue | required | Public mall/market only |
| Date | required | HTML date input |
| Time slot | required | Morning/Afternoon style slots in UI |
| Activity type | required | Shopping, MallVisit, MarketVisit, GroceryRun |
| Special notes | optional | Free text |
| Safety rules accepted | required | Checkbox |
| Share live location | optional | Default on |

## Buddy card (Find buddy)

- Placeholder avatar (ui-avatars.com URL from seed data)
- Display name, age, locality
- Verification badge (Basic / Aadhaar / Police)
- Languages, rating, completed trips, availability
- Bio snippet

## API endpoints (Week 1)

```
POST   /api/auth/login
GET    /api/localities
GET    /api/venues?localityId=
GET    /api/buddies?localityId=&venueId=&activityType=&verifiedOnly=
GET    /api/buddies/{id}
POST   /api/bookings          [JWT]
GET    /api/bookings/mine     [JWT]
```

## Week 1 deliverables

- [x] SQL schema + seed scripts
- [x] Dapper repositories
- [x] Real JWT signing
- [x] Angular wired to API for find/book/list bookings
- [x] Legacy routes hidden
- [x] Repository interfaces (`IUserRepository`, `IBuddyRepository`, …) for future EF swap
- [ ] You: run `database/run-all.bat` on `DESKTOP-J5AN1SG\SQLEXPRESS`
- [ ] You: `dotnet run` + `ng serve --port 4300` end-to-end test

## Week 2 preview

- Admin all-bookings view
- Buddy “my trips” view
- Error/loading polish
- LinkedIn project entry + screenshots
