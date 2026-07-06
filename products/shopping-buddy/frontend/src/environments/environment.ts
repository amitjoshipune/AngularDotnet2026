export const environment = {
  production: false,
  /** When true, MockApiInterceptor handles /api/auth/* without a real backend. */
  useMockAuth: false,
  /** Load buddies, venues, and bookings from SQL-backed .NET API. */
  useShoppingBuddyApi: true,
  /** Relative URL in dev (proxied to .NET API). */
  apiUrl: '/api',
  /** Target for proxy.conf.json — ASP.NET Core default dev port. */
  dotnetApiPort: 5180,
};
