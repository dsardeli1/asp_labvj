import { expect, test } from '@playwright/test';
import { readJson, uniqueLabel } from './test-helpers';

test.describe('Tasks API', () => {
    test('supports CRUD and filtered queries', async ({ request }, testInfo) => {
        const title = uniqueLabel('task', testInfo);
        const updatedTitle = `${title}-updated`;
        let taskId = 0;
        const dueDate = new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString();

        await test.step('List tasks', async () => {
            const response = await request.get('/api/tasks');
            expect(response.status()).toBe(200);
            const tasks = await readJson<Array<{ id: number }>>(response);
            expect(tasks.length).toBeGreaterThan(0);
        });
        await test.step('Fetch a task', async () => {
            const response = await request.get('/api/tasks/1');
            expect(response.status()).toBe(200);
            const task = await readJson<{ id: number; userId: number; categoryId: number }>(response);
            expect(task.id).toBe(1);
            expect(task.userId).toBeGreaterThan(0);
            expect(task.categoryId).toBeGreaterThan(0);
        });
        await test.step('Create a task', async () => {
            const response = await request.post('/api/tasks', { data: { title, description: 'Created by Playwright', dueDate, isCompleted: false, priorityId: 1, userId: 1, categoryId: 1 } });
            expect(response.status()).toBe(201);
            const task = await readJson<{ id: number; title: string }>(response);
            expect(task.title).toBe(title);
            taskId = task.id;
        });
        await test.step('Fetch the created task', async () => {
            const response = await request.get(`/api/tasks/${taskId}`);
            expect(response.status()).toBe(200);
            expect((await readJson<{ id: number }>(response)).id).toBe(taskId);
        });
        await test.step('Update the task', async () => {
            const response = await request.put(`/api/tasks/${taskId}`, { data: { title: updatedTitle, description: 'Updated by Playwright', dueDate, isCompleted: true, priorityId: 2, userId: 1, categoryId: 1 } });
            expect(response.status()).toBe(200);
            const task = await readJson<{ title: string; isCompleted: boolean }>(response);
            expect(task.title).toBe(updatedTitle);
            expect(task.isCompleted).toBe(true);
        });
        await test.step('Read the updated task', async () => {
            const response = await request.get(`/api/tasks/${taskId}`);
            expect(response.status()).toBe(200);
            expect((await readJson<{ title: string }>(response)).title).toBe(updatedTitle);
        });
        await test.step('Read completed tasks', async () => {
            const response = await request.get('/api/tasks/completed');
            expect(response.status()).toBe(200);
            const tasks = await readJson<Array<{ id: number }>>(response);
            expect(tasks.map((task) => task.id)).toContain(taskId);
        });
        await test.step('Read pending tasks', async () => {
            const response = await request.get('/api/tasks/pending');
            expect(response.status()).toBe(200);
            expect((await readJson<Array<unknown>>(response)).length).toBeGreaterThan(0);
        });
        await test.step('Read tasks by category', async () => {
            const response = await request.get('/api/tasks/category/1');
            expect(response.status()).toBe(200);
            const tasks = await readJson<Array<{ id: number }>>(response);
            expect(tasks.map((task) => task.id)).toContain(taskId);
        });
        await test.step('Read tasks by user', async () => {
            const response = await request.get('/api/tasks/user/1');
            expect(response.status()).toBe(200);
            const tasks = await readJson<Array<{ id: number }>>(response);
            expect(tasks.map((task) => task.id)).toContain(taskId);
        });
        await test.step('Delete the task', async () => {
            expect((await request.delete(`/api/tasks/${taskId}`)).status()).toBe(204);
        });
        await test.step('Confirm the task was deleted', async () => {
            expect((await request.get(`/api/tasks/${taskId}`)).status()).toBe(404);
        });
    });
});
