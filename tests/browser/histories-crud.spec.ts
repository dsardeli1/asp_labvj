import { expect, test } from '@playwright/test';
import { chooseAutocomplete, signIn } from './browser-helpers';

test('Histories CRUD is visible in the browser', async ({ page }, testInfo) => {
    const action = `browser-history-${testInfo.workerIndex}-${Date.now()}`;
    const updatedAction = `${action}-updated`;

    await test.step('Sign in as the admin', async () => { await signIn(page); });
    await test.step('Open history creation', async () => {
        await page.goto('/data/histories/create');
        await expect(page.getByRole('heading', { name: 'Create History' })).toBeVisible();
    });
    await test.step('Fill the history form', async () => {
        await chooseAutocomplete(page, 'TaskItemId', 'Complete');
        await page.locator('input[name="Action"]').fill(action);
        await page.locator('[data-datepicker-input]').fill('09/04/2026');
        await page.locator('[data-datepicker-time-input]').fill('12:00');
        await page.keyboard.press('Escape');
    });
    await test.step('Create the history entry', async () => {
        await page.getByRole('button', { name: 'Create' }).click();
        await expect(page).toHaveURL(/\/data\/histories$/);
    });
    await test.step('Verify the history is visible', async () => {
        await expect(page.getByText(action, { exact: true })).toBeVisible();
        await expect(page.getByRole('alert')).toContainText('created successfully');
    });
    await test.step('Open history edit', async () => {
        const row = page.locator('tr').filter({ hasText: action });
        await row.getByRole('link', { name: 'Edit' }).click();
        await expect(page.getByRole('heading', { name: /Edit History/ })).toBeVisible();
    });
    await test.step('Edit the history action', async () => {
        await page.locator('input[name="Action"]').fill(updatedAction);
        await page.getByRole('button', { name: /Save|Update/ }).click();
        await expect(page).toHaveURL(/\/data\/histories$/);
    });
    await test.step('Verify the updated history and feedback', async () => {
        await expect(page.getByText(updatedAction, { exact: true })).toBeVisible();
        await expect(page.getByRole('alert')).toContainText('updated successfully');
    });
    await test.step('Open history delete confirmation', async () => {
        const row = page.locator('tr').filter({ hasText: updatedAction });
        await row.getByRole('link', { name: 'Delete' }).click();
        await expect(page.getByRole('heading', { name: /Delete History/ })).toBeVisible();
    });
    await test.step('Delete the history entry', async () => {
        await page.getByRole('button', { name: 'Delete' }).click();
        await expect(page).toHaveURL(/\/data\/histories$/);
    });
    await test.step('Verify the history was deleted', async () => {
        await expect(page.getByRole('alert')).toContainText('deleted successfully');
        await expect(page.getByText(updatedAction, { exact: true })).not.toBeVisible();
    });
});
