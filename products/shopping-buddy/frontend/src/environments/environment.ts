export const environment = {
  production: false,
  /** When true, MockApiInterceptor handles /api/auth/* without a real backend. */
  useMockAuth: false,
  /** Relative URL in dev (proxied to .NET API). Absolute URL in production if needed. */
  apiUrl: '/api',
  /** Target for proxy.conf.json — ASP.NET Core default dev port. */
  dotnetApiPort: 5180,
};
