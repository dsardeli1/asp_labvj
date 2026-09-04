import { expect, test } from '@playwright/test';
import { readJson, uniqueLabel } from './test-helpers';

test.describe('Histories API', () => {
    test('supports CRUD and task validation', async ({ request }, testInfo) => {
        const action = uniqueLabel('history', testInfo);
        const updatedAction = `${action}-updated`;
        const actionDate = new Date(Date.now() + 3 * 24 * 60 * 60 * 1000).toISOString();
        let historyId = 0;

        await test.step('List histories', async () => {
            const response = await request.get('/api/histories');
            expect(response.status()).toBe(200);
            expect((await readJson<Array<unknown>>(response)).length).toBeGreaterThan(0);
        });
        await test.step('Fetch a history', async () => {
            const response = await request.get('/api/histories/1');
            expect(response.status()).toBe(200);
            expect((await readJson<{ id: number }>(response)).id).toBe(1);
        });
        await test.step('Create a history entry', async () => {
            const response = await request.post('/api/histories', { data: { action, actionDate, taskItemId: 1 } });
            expect(response.status()).toBe(201);
            const history = await readJson<{ id: number; action: string }>(response);
            expect(history.action).toBe(action);
            historyId = history.id;
        });
        await test.step('Fetch the created history', async () => {
            const response = await request.get(`/api/histories/${historyId}`);
            expect(response.status()).toBe(200);
            expect((await readJson<{ id: number }>(response)).id).toBe(historyId);
        });
        await test.step('Update the history entry', async () => {
            const response = await request.put(`/api/histories/${historyId}`, { data: { action: updatedAction, actionDate, taskItemId: 1 } });
            expect(response.status()).toBe(200);
            expect((await readJson<{ action: string }>(response)).action).toBe(updatedAction);
        });
        await test.step('Read the updated history', async () => {
            const response = await request.get(`/api/histories/${historyId}`);
            expect(response.status()).toBe(200);
            expect((await readJson<{ action: string }>(response)).action).toBe(updatedAction);
        });
        await test.step('Reject an unknown task on create', async () => {
            const response = await request.post('/api/histories', { data: { action: 'Invalid', actionDate, taskItemId: 9999 } });
            expect(response.status()).toBe(400);
            expect(await response.text()).toContain('Task 9999 was not found');
        });
        await test.step('Reject an unknown history', async () => {
            expect((await request.get('/api/histories/9999')).status()).toBe(404);
        });
        await test.step('Reject updating an unknown history', async () => {
            expect((await request.put('/api/histories/9999', { data: { action: 'Invalid', actionDate, taskItemId: 1 } })).status()).toBe(404);
        });
        await test.step('Delete the history entry', async () => {
            expect((await request.delete(`/api/histories/${historyId}`)).status()).toBe(204);
        });
        await test.step('Confirm the history was deleted', async () => {
            expect((await request.get(`/api/histories/${historyId}`)).status()).toBe(404);
        });
    });
});
