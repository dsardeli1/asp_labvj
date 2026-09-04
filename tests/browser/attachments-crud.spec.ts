import { expect, test } from '@playwright/test';
import { chooseAutocomplete, signIn } from './browser-helpers';

test('Attachments CRUD is visible in the browser', async ({ page }, testInfo) => {
    const fileName = `browser-attachment-${testInfo.workerIndex}-${Date.now()}.txt`;
    const updatedFileName = `updated-${fileName}`;

    await test.step('Sign in as the admin', async () => { await signIn(page); });
    await test.step('Open attachment creation', async () => {
        await page.goto('/data/attachments/create');
        await expect(page.getByRole('heading', { name: 'Create Attachment' })).toBeVisible();
    });
    await test.step('Fill the attachment form', async () => {
        await page.locator('input[name="FileName"]').fill(fileName);
        await page.locator('input[name="FilePath"]').fill(`/mock-files/${fileName}`);
        await chooseAutocomplete(page, 'TaskItemId', 'Complete');
    });
    await test.step('Create the attachment', async () => {
        await page.getByRole('button', { name: 'Create' }).click();
        await expect(page).toHaveURL(/\/data\/attachments$/);
    });
    await test.step('Verify the attachment is visible', async () => {
        await expect(page.getByText(fileName, { exact: true })).toBeVisible();
        await expect(page.getByRole('alert')).toContainText('created successfully');
    });
    await test.step('Open attachment edit', async () => {
        const row = page.locator('tr').filter({ hasText: fileName });
        await row.getByRole('link', { name: 'Edit' }).click();
        await expect(page.getByRole('heading', { name: /Edit Attachment/ })).toBeVisible();
    });
    await test.step('Edit the attachment name and path', async () => {
        await page.locator('input[name="FileName"]').fill(updatedFileName);
        await page.locator('input[name="FilePath"]').fill(`/mock-files/${updatedFileName}`);
        await page.getByRole('button', { name: /Save|Update/ }).click();
        await expect(page).toHaveURL(/\/data\/attachments$/);
    });
    await test.step('Verify the updated attachment and feedback', async () => {
        await expect(page.getByText(updatedFileName, { exact: true })).toBeVisible();
        await expect(page.getByRole('alert')).toContainText('updated successfully');
    });
    await test.step('Open attachment delete confirmation', async () => {
        const row = page.locator('tr').filter({ hasText: updatedFileName });
        await row.getByRole('link', { name: 'Delete' }).click();
        await expect(page.getByRole('heading', { name: /Delete Attachment/ })).toBeVisible();
    });
    await test.step('Delete the attachment', async () => {
        await page.getByRole('button', { name: 'Delete' }).click();
        await expect(page).toHaveURL(/\/data\/attachments$/);
    });
    await test.step('Verify the attachment was deleted', async () => {
        await expect(page.getByRole('alert')).toContainText('deleted successfully');
        await expect(page.getByText(updatedFileName, { exact: true })).not.toBeVisible();
    });
});
