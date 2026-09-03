import { expect, test } from '@playwright/test';
import { readJson, uniqueLabel } from './test-helpers';

test.describe('Comments API', () => {
    test('supports CRUD, task filtering, and validation', async ({ request }, testInfo) => {
        const content = uniqueLabel('comment', testInfo);
        const updatedContent = `${content}-updated`;
        let commentId = 0;

        await test.step('List comments', async () => {
            const response = await request.get('/api/comments');
            expect(response.status()).toBe(200);
            expect((await readJson<Array<unknown>>(response)).length).toBeGreaterThan(0);
        });
        await test.step('Fetch a seeded comment', async () => {
            const response = await request.get('/api/comments/1');
            expect(response.status()).toBe(200);
            expect((await readJson<{ id: number }>(response)).id).toBe(1);
        });
        await test.step('Create a comment', async () => {
            const response = await request.post('/api/comments', { data: { content, taskItemId: 1, userId: 1 } });
            expect(response.status()).toBe(201);
            const comment = await readJson<{ id: number; content: string; isEdited: boolean }>(response);
            expect(comment.content).toBe(content);
            expect(comment.isEdited).toBe(false);
            commentId = comment.id;
        });
        await test.step('Fetch the created comment', async () => {
            const response = await request.get(`/api/comments/${commentId}`);
            expect(response.status()).toBe(200);
            expect((await readJson<{ id: number }>(response)).id).toBe(commentId);
        });
        await test.step('List comments by task', async () => {
            const response = await request.get('/api/comments/task/1');
            expect(response.status()).toBe(200);
            const comments = await readJson<Array<{ id: number }>>(response);
            expect(comments.map((comment) => comment.id)).toContain(commentId);
        });
        await test.step('Update the comment', async () => {
            const response = await request.put(`/api/comments/${commentId}`, { data: { content: updatedContent, taskItemId: 1, userId: 1 } });
            expect(response.status()).toBe(200);
            expect((await readJson<{ content: string }>(response)).content).toBe(updatedContent);
        });
        await test.step('Read the updated comment', async () => {
            const response = await request.get(`/api/comments/${commentId}`);
            expect(response.status()).toBe(200);
            expect((await readJson<{ content: string }>(response)).content).toBe(updatedContent);
        });
        await test.step('Reject an unknown task', async () => {
            const response = await request.post('/api/comments', { data: { content: 'Invalid', taskItemId: 9999, userId: 1 } });
            expect(response.status()).toBe(400);
            expect(await response.text()).toContain('Task 9999 was not found');
        });
        await test.step('Reject an unknown user', async () => {
            const response = await request.post('/api/comments', { data: { content: 'Invalid', taskItemId: 1, userId: 9999 } });
            expect(response.status()).toBe(400);
            expect(await response.text()).toContain('User 9999 was not found');
        });
        await test.step('Reject fetching an unknown comment', async () => {
            expect((await request.get('/api/comments/9999')).status()).toBe(404);
        });
        await test.step('Delete the comment', async () => {
            expect((await request.delete(`/api/comments/${commentId}`)).status()).toBe(204);
        });
        await test.step('Confirm the comment was deleted', async () => {
            expect((await request.get(`/api/comments/${commentId}`)).status()).toBe(404);
        });
    });
});
