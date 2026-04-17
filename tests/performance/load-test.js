import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '30s', target: 20 }, // ramp up
        { duration: '1m', target: 50 },  // expected load // could be higher but is lower for demonstrating purposes only
        { duration: '30s', target: 50 }, // keep steady load
        { duration: '20s', target: 0 }   // ramp down
    ],
    thresholds: {
        http_req_failed: ['rate<0.01'],
        http_req_duration: ['p(95)<500']
    }
};

export default function () {
    const response = http.get(`${__ENV.BASE_URL}/api/products`);

    check(response, {
        'status is 200': r => r.status === 200,
        'content type is json': r => (r.headers['Content-Type'] || '').includes('application/json')
    });

    sleep(1);
}