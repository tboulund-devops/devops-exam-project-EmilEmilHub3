import http from 'k6/http';
import { check, sleep } from 'k6';

/**
 * Soak Test
 *
 * Purpose:
 * - Verifies that the API remains stable under sustained traffic over time.
 * - Helps detect issues such as gradual slowdown, memory leaks, or resource exhaustion.
 *
 * Test profile:
 * - Gradual ramp-up
 * - Long-running steady load
 * - Controlled ramp-down
 *
 * Notes:
 * - This is a short, pipeline-friendly soak test.
 * - BASE_URL is read from the environment when running in CI/CD.
 * - Falls back to localhost for local execution.
 */
const BASE_URL = __ENV.BASE_URL || 'http://localhost:8000';

export const options = {
    stages: [
        { duration: '1m', target: 8 },    // Ramp up slowly
        { duration: '15m', target: 8 },   // Sustain load over time
        { duration: '1m', target: 0 }     // Ramp down
    ],
    thresholds: {
        http_req_failed: ['rate<0.01'],
        http_req_duration: ['p(95)<700'],
        checks: ['rate>0.99']
    }
};

/**
 * Executes one iteration of the soak test scenario.
 */
export default function () {
    const response = http.get(`${BASE_URL}/api/products`);

    check(response, {
        'GET /api/products status is 200': (r) => r.status === 200,
        'GET /api/products response time < 700ms': (r) => r.timings.duration < 700
    });

    sleep(1);
}