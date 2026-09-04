import { expect, test } from '@playwright/test';
import { signIn } from './browser-helpers';

test('Categories CRUD is visible in the browser', async ({ page }, testInfo) => {
    const name = `browser-category-${testInfo.workerIndex}-${Date.now()}`;
    const updatedName = `${name}-updated`;

    await test.step('Sign in as the admin', async () => { await signIn(page); });
    await test.step('Open category creation', async () => {
        await page.goto('/data/categories/create');
        await expect(page.getByRole('heading', { name: 'Create Category' })).toBeVisible();
    });
    await test.step('Fill the category form', async () => {
        await page.locator('input[name="Name"]').fill(name);
        await page.locator('textarea[name="Description"]').fill('Created in the visible browser flow');
        await page.locator('input[name="Color"]').fill('#123abc');
    });
    await test.step('Create the category', async () => {
        await page.getByRole('button', { name: 'Create' }).click();
        await expect(page).toHaveURL(/\/data\/categories$/);
    });
    await test.step('Verify the category is visible', async () => {
        await expect(page.getByText(name, { exact: true })).toBeVisible();
        await expect(page.getByRole('alert')).toContainText('created successfully');
    });
    await test.step('Open category edit', async () => {
        const row = page.locator('tr').filter({ hasText: name });
        await row.getByRole('link', { name: 'Edit' }).click();
        await expect(page.getByRole('heading', { name: /Edit Category/ })).toBeVisible();
    });
    await test.step('Edit the category name', async () => {
        await page.locator('input[name="Name"]').fill(updatedName);
        await page.getByRole('button', { name: 'Save changes' }).click();
        await expect(page).toHaveURL(/\/data\/categories$/);
    });
    await test.step('Verify the updated category and feedback', async () => {
        await expect(page.getByText(updatedName, { exact: true })).toBeVisible();
        await expect(page.getByRole('alert')).toContainText('updated successfully');
    });
    await test.step('Open category delete confirmation', async () => {
        const row = page.locator('tr').filter({ hasText: updatedName });
        await row.getByRole('link', { name: 'Delete' }).click();
        await expect(page.getByRole('heading', { name: /Delete Category/ })).toBeVisible();
    });
    await test.step('Delete the category', async () => {
        await page.getByRole('button', { name: 'Delete' }).click();
        await expect(page).toHaveURL(/\/data\/categories$/);
    });
    await test.step('Verify the category was deleted', async () => {
        await expect(page.getByRole('alert')).toContainText('deleted successfully');
        await expect(page.getByText(updatedName, { exact: true })).not.toBeVisible();
    });
});
