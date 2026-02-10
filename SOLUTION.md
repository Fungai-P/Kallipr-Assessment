The system would run on AWS with an ASP.NET Core API deployed on AWS EC2 behind Elastic Load balancer (EBL). The React frontend would be hosted on S3 and CloudFront. Telemetry events would be buffered using SQS and stored in RDS PostgreSQL, with Redis used for rate limiting and caching. CloudWatch would handle logging, metrics, and alerts.

Tenant isolation would be enforced in middleware, with the tenant resolved from a JWT claim or device identity. All tables include a TenantId, and all queries and indexes are scoped by tenant. A shared database is used by default, with stronger isolation options available if needed.

High traffic and bursts are handled by queueing events in SQS and processing them asynchronously. Rate limits protect the API, and autoscaling consumers handle increased load. Duplicate events are prevented using a unique (TenantId, DeviceId, EventId) key.

CI/CD runs builds, tests, and basic checks on pull requests. Merges to main build and deploy containers via ECR to staging and production using rolling or blue-green deployments.

Schema changes use EF Core migrations with backward-compatible updates. New fields are added first, data is backfilled, and old structures are removed only after safe rollout.