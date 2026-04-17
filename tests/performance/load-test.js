import http from 'k6/http';
import { check, sleep } from 'k6';

/**
 * Spike Test
 *
 * Purpose:
 * - Verifies how the API behaves during a sudden surge in traffic.
 * - Evaluates whether the system remains stable and responsive during short traffic bursts.
 *
 * Test profile:
 * - Small baseline traffic
 * - Rapid spike to a level much higher than normal load
 * - Short hold period
 * - Quick recovery
 *
 * Notes:
 * - This is a pipeline-friendly spike test.
 * - BASE_URL is read from the environment when running in CI/CD.
 * - Falls back to localhost for local execution.
 */
const BASE_URL = __ENV.BASE_URL || 'http://localhost:8000';

export const options = {
    stages: [
        { duration: '20s', target: 5 },   // Small baseline
        { duration: '5s', target: 60 },   // Fast spike well above normal load
        { duration: '20s', target: 60 },  // Hold spike briefly
        { duration: '10s', target: 5 },   // Recover quickly
        { duration: '10s', target: 0 }    // End test
    ],
    thresholds: {
        http_req_failed: ['rate<0.05'],
        http_req_duration: ['p(95)<1000'],
        checks: ['rate>0.95']
    }
};

/**
 * Executes one iteration of the spike test scenario.
 */
export default function () {
    const response = http.get(`${BASE_URL}/api/products`);

    check(response, {
        'GET /api/products status is 200': (r) => r.status === 200,
        'GET /api/products response time < 1000ms': (r) => r.timings.duration < 1000
    });

    sleep(1);
}