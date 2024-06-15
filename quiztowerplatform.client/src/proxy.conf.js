const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:54218';

const PROXY_CONFIG = [
  {
    context: [
      // BFF Management Routes
      "/bff",

      // OIDC Handler Endpoints
      "/signin-oidc",
      "/signout-callback-oidc",

      // Local API Endpoints
      "/api/WeatherForecastAbc",

      // Remote Tower of Quizzes (TOQ) API Endpoints
      "/toq"
    ],
    target: target,
    secure: true,
    changeOrigin: true,
    logLevel: "debug"    
  }
]

module.exports = PROXY_CONFIG;
