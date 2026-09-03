import { expect, test } from '@playwright/test';
import { readJson, uniqueLabel } from './test-helpers';

test.describe('Attachments API', () => {
    test('supports CRUD and task validation', async ({ request }, testInfo) => {
        const fileName = `${uniqueLabel('attachment', testInfo)}.txt`;
        const updatedFileName = `updated-${fileName}`;
        let attachmentId = 0;

        await test.step('List attachments', async () => {
            const response = await request.get('/api/attachments');
            expect(response.status()).toBe(200);
            expect((await readJson<Array<unknown>>(response)).length).toBeGreaterThan(0);
        });
        await test.step('Fetch a seeded attachment', async () => {
            const response = await request.get('/api/attachments/1');
            expect(response.status()).toBe(200);
            expect((await readJson<{ id: number }>(response)).id).toBe(1);
        });
        await test.step('Create an attachment', async () => {
            const response = await request.post('/api/attachments', { data: { fileName, filePath: `/mock-files/${fileName}`, taskItemId: 1 } });
            expect(response.status()).toBe(201);
            const attachment = await readJson<{ id: number; fileName: string }>(response);
            expect(attachment.fileName).toBe(fileName);
            attachmentId = attachment.id;
        });
        await test.step('Fetch the created attachment', async () => {
            const response = await request.get(`/api/attachments/${attachmentId}`);
            expect(response.status()).toBe(200);
            expect((await readJson<{ id: number }>(response)).id).toBe(attachmentId);
        });
        await test.step('Update the attachment', async () => {
            const response = await request.put(`/api/attachments/${attachmentId}`, { data: { fileName: updatedFileName, filePath: `/mock-files/${updatedFileName}`, taskItemId: 1 } });
            expect(response.status()).toBe(200);
            const attachment = await readJson<{ fileName: string; filePath: string }>(response);
            expect(attachment.fileName).toBe(updatedFileName);
            expect(attachment.filePath).toBe(`/mock-files/${updatedFileName}`);
        });
        await test.step('Read the updated attachment', async () => {
            const response = await request.get(`/api/attachments/${attachmentId}`);
            expect(response.status()).toBe(200);
            expect((await readJson<{ fileName: string }>(response)).fileName).toBe(updatedFileName);
        });
        await test.step('Reject an unknown task on create', async () => {
            const response = await request.post('/api/attachments', { data: { fileName: 'invalid.txt', filePath: '/mock-files/invalid.txt', taskItemId: 9999 } });
            expect(response.status()).toBe(400);
            expect(await response.text()).toContain('Task 9999 was not found');
        });
        await test.step('Reject an unknown attachment', async () => {
            expect((await request.get('/api/attachments/9999')).status()).toBe(404);
        });
        await test.step('Reject updating an unknown attachment', async () => {
            expect((await request.put('/api/attachments/9999', { data: { fileName: 'invalid.txt', filePath: '/mock-files/invalid.txt', taskItemId: 1 } })).status()).toBe(404);
        });
        await test.step('Delete the attachment', async () => {
            expect((await request.delete(`/api/attachments/${attachmentId}`)).status()).toBe(204);
        });
        await test.step('Confirm the attachment was deleted', async () => {
            expect((await request.get(`/api/attachments/${attachmentId}`)).status()).toBe(404);
        });
    });
});
