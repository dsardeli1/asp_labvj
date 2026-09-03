import { expect, test } from '@playwright/test';
import { readJson, uniqueLabel } from './test-helpers';

test.describe('Categories API', () => {
    test('supports the full category lifecycle and validation', async ({ request }, testInfo) => {
        const name = uniqueLabel('category', testInfo);
        const updatedName = `${name}-updated`;
        let categoryId = 0;

        await test.step('List categories', async () => {
            const response = await request.get('/api/categories');
            expect(response.status()).toBe(200);
            const categories = await readJson<Array<{ id: number }>>(response);
            expect(categories.length).toBeGreaterThan(0);
        });
        await test.step('Fetch a seeded category', async () => {
            const response = await request.get('/api/categories/1');
            expect(response.status()).toBe(200);
            const category = await readJson<{ id: number; name: string }>(response);
            expect(category.id).toBe(1);
            expect(category.name).toBe('Planning');
        });
        await test.step('Create a category', async () => {
            const response = await request.post('/api/categories', { data: { name, description: 'Created by Playwright', color: '#123abc', isActive: true } });
            expect(response.status()).toBe(201);
            const category = await readJson<{ id: number; name: string }>(response);
            expect(category.name).toBe(name);
            categoryId = category.id;
        });
        await test.step('Fetch the created category', async () => {
            const response = await request.get(`/api/categories/${categoryId}`);
            expect(response.status()).toBe(200);
            expect((await readJson<{ id: number }>(response)).id).toBe(categoryId);
        });
        await test.step('Update the category', async () => {
            const response = await request.put(`/api/categories/${categoryId}`, { data: { name: updatedName, description: 'Updated by Playwright', color: '#654321', isActive: false } });
            expect(response.status()).toBe(200);
            const category = await readJson<{ name: string; isActive: boolean }>(response);
            expect(category.name).toBe(updatedName);
            expect(category.isActive).toBe(false);
        });
        await test.step('Read the updated category', async () => {
            const response = await request.get(`/api/categories/${categoryId}`);
            expect(response.status()).toBe(200);
            expect((await readJson<{ name: string }>(response)).name).toBe(updatedName);
        });
        await test.step('Reject a duplicate category name', async () => {
            const response = await request.post('/api/categories', { data: { name: 'Planning', description: 'Duplicate', color: '#abcdef', isActive: true } });
            expect(response.status()).toBe(400);
            expect(await response.text()).toContain('already exists');
        });
        await test.step('Reject an invalid category color', async () => {
            const response = await request.post('/api/categories', { data: { name: uniqueLabel('invalid', testInfo), description: 'Invalid', color: 'blue', isActive: true } });
            expect(response.status()).toBe(400);
        });
        await test.step('Delete the created category', async () => {
            expect((await request.delete(`/api/categories/${categoryId}`)).status()).toBe(204);
        });
        await test.step('Confirm the category was deleted', async () => {
            expect((await request.get(`/api/categories/${categoryId}`)).status()).toBe(404);
        });
        await test.step('Reject deleting a category assigned to tasks', async () => {
            const response = await request.delete('/api/categories/1');
            expect(response.status()).toBe(409);
            expect(await response.text()).toContain('cannot be deleted');
        });
    });
});
