import { expect, test } from '@playwright/test';
import { chooseAutocomplete, signIn } from './browser-helpers';

test('Tasks CRUD is visible in the browser', async ({ page }, testInfo) => {
    const title = `browser-task-${testInfo.workerIndex}-${Date.now()}`;
    const updatedTitle = `${title}-updated`;

    await test.step('Sign in as the admin', async () => { await signIn(page); });
    await test.step('Open task creation', async () => {
        await page.goto('/data/tasks/create');
        await expect(page.getByRole('heading', { name: 'Create Task' })).toBeVisible();
    });
    await test.step('Fill the task form', async () => {
        await page.locator('input[name="Title"]').fill(title);
        await page.locator('textarea[name="Description"]').fill('Created in the visible browser flow');
        await page.locator('select[name="PriorityId"]').selectOption('1');
        await chooseAutocomplete(page, 'UserId', 'ana');
        await chooseAutocomplete(page, 'CategoryId', 'Planning');
        await page.locator('[data-datepicker-input]').fill('09/11/2026');
        await page.keyboard.press('Escape');
    });
    await test.step('Create the task', async () => {
        await page.getByRole('button', { name: 'Create' }).click();
        await expect(page).toHaveURL(/\/data\/tasks$/);
    });
    await test.step('Verify the task is visible', async () => {
        await expect(page.getByText(title, { exact: true })).toBeVisible();
        await expect(page.getByRole('alert')).toContainText('created successfully');
    });
    await test.step('Open task edit', async () => {
        const row = page.locator('tr').filter({ hasText: title });
        await row.getByRole('link', { name: 'Edit' }).click();
        await expect(page.getByRole('heading', { name: /Edit Task/ })).toBeVisible();
    });
    await test.step('Edit the task title', async () => {
        await page.locator('input[name="Title"]').fill(updatedTitle);
        await page.getByRole('button', { name: /Save|Update/ }).click();
        await expect(page).toHaveURL(/\/data\/tasks$/);
    });
    await test.step('Verify the updated task and feedback', async () => {
        await expect(page.getByText(updatedTitle, { exact: true })).toBeVisible();
        await expect(page.getByRole('alert')).toContainText('updated successfully');
    });
    await test.step('Open task delete confirmation', async () => {
        const row = page.locator('tr').filter({ hasText: updatedTitle });
        await row.getByRole('link', { name: 'Delete' }).click();
        await expect(page.getByRole('heading', { name: /Delete Task/ })).toBeVisible();
    });
    await test.step('Delete the task', async () => {
        await page.getByRole('button', { name: 'Delete' }).click();
        await expect(page).toHaveURL(/\/data\/tasks$/);
    });
    await test.step('Verify the task was deleted', async () => {
        await expect(page.getByRole('alert')).toContainText('deleted successfully');
        await expect(page.getByText(updatedTitle, { exact: true })).not.toBeVisible();
    });
});
