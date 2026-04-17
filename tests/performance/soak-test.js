import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '1m', target: 10 },   // Ramp up
        { duration: '10m', target: 10 },  // Stay under steady load for a long period
        { duration: '1m', target: 0 }     // Ramp down
    ],
    thresholds: {
        http_req_failed: ['rate<0.01'],
        http_req_duration: ['p(95)<700'],
        checks: ['rate>0.99']
    }
};

const BASE_URL = __ENV.BASE_URL || 'http://localhost:8000';

export default function () {
    const response = http.get(`${BASE_URL}/api/products`);

    check(response, {
        'GET /api/products status is 200': (r) => r.status === 200,
        'GET /api/products response time < 700ms': (r) => r.timings.duration < 700
    });

    sleep(1);
}