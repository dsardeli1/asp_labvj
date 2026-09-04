import { expect, test } from '@playwright/test';
import { chooseAutocomplete, signIn } from './browser-helpers';

test('Comments CRUD is visible in the browser', async ({ page }, testInfo) => {
    const content = `browser-comment-${testInfo.workerIndex}-${Date.now()}`;
    const updatedContent = `${content}-updated`;

    await test.step('Sign in as the admin', async () => { await signIn(page); });
    await test.step('Open comment creation', async () => {
        await page.goto('/data/comments/create');
        await expect(page.getByRole('heading', { name: 'Create Comment' })).toBeVisible();
    });
    await test.step('Fill the comment form', async () => {
        await chooseAutocomplete(page, 'TaskItemId', 'Complete');
        await chooseAutocomplete(page, 'UserId', 'ana');
        await page.locator('textarea[name="Content"]').fill(content);
    });
    await test.step('Create the comment', async () => {
        await page.getByRole('button', { name: 'Create' }).click();
        await expect(page).toHaveURL(/\/data\/comments$/);
    });
    await test.step('Verify the comment is visible', async () => {
        await expect(page.getByText(content, { exact: true })).toBeVisible();
        await expect(page.getByRole('alert')).toContainText('created successfully');
    });
    await test.step('Open comment edit', async () => {
        const row = page.locator('tr').filter({ hasText: content });
        await row.getByRole('link', { name: 'Edit' }).click();
        await expect(page.getByRole('heading', { name: /Edit Comment/ })).toBeVisible();
    });
    await test.step('Edit the comment', async () => {
        await page.locator('textarea[name="Content"]').fill(updatedContent);
        await page.getByRole('button', { name: /Save|Update/ }).click();
        await expect(page).toHaveURL(/\/data\/comments$/);
    });
    await test.step('Verify the updated comment and feedback', async () => {
        await expect(page.getByText(updatedContent, { exact: true })).toBeVisible();
        await expect(page.getByRole('alert')).toContainText('updated successfully');
    });
    await test.step('Open comment delete confirmation', async () => {
        const row = page.locator('tr').filter({ hasText: updatedContent });
        await row.getByRole('link', { name: 'Delete' }).click();
        await expect(page.getByRole('heading', { name: /Delete Comment/ })).toBeVisible();
    });
    await test.step('Delete the comment', async () => {
        await page.getByRole('button', { name: 'Delete' }).click();
        await expect(page).toHaveURL(/\/data\/comments$/);
    });
    await test.step('Verify the comment was deleted', async () => {
        await expect(page.getByRole('alert')).toContainText('deleted successfully');
        await expect(page.getByText(updatedContent, { exact: true })).not.toBeVisible();
    });
});
