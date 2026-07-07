# Roles and booking workflow (MVP)

## User types

| Role | Can book buddies? | Can accept/reject requests? | Admin panel |
|------|-------------------|----------------------------|-------------|
| **Customer** | Yes | No | No |
| **Buddy** | Yes (if also Customer) | Yes (own buddy profile) | No |
| **Admin** | No (MVP) | No | Yes (later) |

## Registration model (decided for MVP)

1. **New person registers** → gets **Customer** only.
2. **Apply to become a Buddy** → admin verifies → add **Buddy** role + `Buddies` profile row (Week 2+ UI; today: seed SQL).
3. **Buddy users also get Customer** → Meera can book another buddy and accept her own incoming requests.

We do **not** assign Buddy + Admin together. Admin is separate demo accounts.

## Booking lifecycle

```
Customer books → PendingBuddy
       ↓
Buddy confirms → Confirmed
Buddy rejects  → RejectedByBuddy (+ reason code / free text)
```

## Duplicate rule

Same **customer** cannot book the same **venue + date + time slot** twice while a booking is active (`PendingBuddy` or `Confirmed`).

## Rejection reason codes

| Code | Label |
|------|--------|
| ScheduleConflict | Already booked at that time |
| VenueTooFar | Venue is too far for me |
| PersonalEmergency | Personal / family emergency |
| SafetyConcern | Safety concern about request |
| Other | Other (free text required) |

## Demo logins after 007 migration

| Email | Roles |
|-------|--------|
| customer@demo.com | Customer |
| senior@demo.com | Customer |
| meera@demo.com | Buddy, Customer |
| anjali@demo.com | Buddy, Customer |
| admin@example.com | Admin |
