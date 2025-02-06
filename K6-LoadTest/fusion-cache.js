import http from 'k6/http';
import { check, sleep } from 'k6';
import { textSummary } from 'https://jslib.k6.io/k6-summary/0.0.2/index.js';
export let options = {
    stages: [
        { duration: '2s', target: 100 },     // Ramp up to 100 users
        { duration: '2s', target: 1000 },    // Ramp up to 1000 users
        { duration: '2s', target: 0 },       // Ramp down to 0 users
    ],
    thresholds: {
        http_req_duration: ['p(95)<2000'], // 95% of requests should be below 2s
        http_req_failed: ['rate<0.1'],     // Less than 10% of requests should fail
    }
};

export default function () {
    let res = http.get('http://localhost:5278/fusion-cache');
    check(res, {
        'status is 200': (r) => r.status === 200,
    });
    sleep(1);  // Add a 1s pause between iterations
}

export function handleSummary(data) {
    return {
        'K6-LoadTest/results/fusion-cache-summary.txt': textSummary(data, { indent: ' ', enableColors: false }),
        stdout: textSummary(data, { indent: ' ', enableColors: true }),
    };
}