import { expect, test } from '@playwright/test';
import { readJson, uniqueLabel } from './test-helpers';

test.describe('Users API', () => {
    test('supports the full lifecycle and filtered reads', async ({ request }, testInfo) => {
        const username = uniqueLabel('user', testInfo);
        const email = `${username}@example.com`;
        let userId = 0;

        await test.step('List users', async () => {
            const response = await request.get('/api/users');
            expect(response.status()).toBe(200);
            const users = await readJson<Array<{ id: number }>>(response);
            expect(users.length).toBeGreaterThan(0);
        });
        await test.step('Fetch a seeded user', async () => {
            const response = await request.get('/api/users/1');
            expect(response.status()).toBe(200);
            const user = await readJson<{ id: number; username: string }>(response);
            expect(user.id).toBe(1);
            expect(user.username).toBe('ana.kovacic');
        });
        await test.step('Create a user', async () => {
            const response = await request.post('/api/users', { data: { username, email, passwordHash: 'Password123!Aa', firstName: 'Test', lastName: 'User' } });
            expect(response.status()).toBe(201);
            const user = await readJson<{ id: number; username: string; email: string }>(response);
            expect(user.username).toBe(username);
            expect(user.email).toBe(email);
            userId = user.id;
        });
        await test.step('Fetch the created user', async () => {
            const response = await request.get(`/api/users/${userId}`);
            expect(response.status()).toBe(200);
            expect((await readJson<{ id: number }>(response)).id).toBe(userId);
        });
        await test.step('Update the user', async () => {
            const response = await request.put(`/api/users/${userId}`, { data: { username, email: `${username}.updated@example.com`, passwordHash: 'Password123!Bb', firstName: 'Updated', lastName: 'Person' } });
            expect(response.status()).toBe(200);
            const user = await readJson<{ firstName: string; lastName: string }>(response);
            expect(user.firstName).toBe('Updated');
            expect(user.lastName).toBe('Person');
        });
        await test.step('Read users with tasks', async () => {
            const response = await request.get('/api/users/with-tasks');
            expect(response.status()).toBe(200);
            const users = await readJson<Array<{ id: number }>>(response);
            expect(users.map((user) => user.id)).toEqual(expect.arrayContaining([1, 2]));
        });
        await test.step('Read users without tasks', async () => {
            const response = await request.get('/api/users/without-tasks');
            expect(response.status()).toBe(200);
            const users = await readJson<Array<{ id: number }>>(response);
            expect(users.map((user) => user.id)).toContain(3);
        });
        await test.step('Reject fetching an unknown user', async () => {
            expect((await request.get('/api/users/9999')).status()).toBe(404);
        });
        await test.step('Reject updating an unknown user', async () => {
            const response = await request.put('/api/users/9999', { data: { username: 'missing', email: 'missing@example.com', passwordHash: 'Password123!Aa', firstName: 'Missing', lastName: 'User' } });
            expect(response.status()).toBe(404);
        });
        await test.step('Delete the created user', async () => {
            expect((await request.delete(`/api/users/${userId}`)).status()).toBe(204);
        });
        await test.step('Confirm the user was deleted', async () => {
            expect((await request.get(`/api/users/${userId}`)).status()).toBe(404);
        });
        await test.step('Reject deleting a user assigned to tasks', async () => {
            const response = await request.delete('/api/users/1');
            expect(response.status()).toBe(409);
            expect(await response.text()).toContain('cannot be deleted');
        });
    });
});
