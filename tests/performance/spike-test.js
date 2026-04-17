import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '15s', target: 5 },   // Small baseline
        { duration: '10s', target: 50 },  // Sudden spike
        { duration: '20s', target: 50 },  // Hold spike shortly
        { duration: '10s', target: 5 },   // Drop down again
        { duration: '10s', target: 0 }    // End test
    ],
    thresholds: {
        http_req_failed: ['rate<0.05'],
        http_req_duration: ['p(95)<1000'],
        checks: ['rate>0.95']
    }
};

const BASE_URL = __ENV.BASE_URL || 'http://localhost:8000';

export default function () {
    const response = http.get(`${BASE_URL}/api/products`);

    check(response, {
        'GET /api/products status is 200': (r) => r.status === 200,
        'GET /api/products response time < 1000ms': (r) => r.timings.duration < 1000
    });

    sleep(1);
}