import { expect, test } from '@playwright/test';
import { signIn } from './browser-helpers';

test('Users CRUD is visible in the browser', async ({ page }, testInfo) => {
    const username = `browser-user-${testInfo.workerIndex}-${Date.now()}`;
    const email = `${username}@example.com`;

    await test.step('Sign in as the admin', async () => {
        await signIn(page);
    });
    await test.step('Open the user creation page', async () => {
        await page.goto('/data/users/create');
        await expect(page.getByRole('heading', { name: 'Create User' })).toBeVisible();
    });
    await test.step('Fill the new user form', async () => {
        await page.locator('input[name="Username"]').fill(username);
        await page.locator('input[name="Email"]').fill(email);
        await page.locator('input[name="FirstName"]').fill('Browser');
        await page.locator('input[name="LastName"]').fill('User');
        await page.locator('input[name="PasswordHash"]').fill('Password123!Aa');
    });
    await test.step('Submit the create form', async () => {
        await page.getByRole('button', { name: 'Create' }).click();
        await expect(page).toHaveURL(/\/data\/users$/);
    });
    await test.step('Verify the created user is visible', async () => {
        await expect(page.getByText(username, { exact: true })).toBeVisible();
        await expect(page.getByRole('alert')).toContainText('User created');
    });
    await test.step('Open the user edit page', async () => {
        const row = page.locator('tr').filter({ hasText: username });
        await row.getByRole('link', { name: 'Edit' }).click();
        await expect(page.getByRole('heading', { name: /Edit User/ })).toBeVisible();
    });
    await test.step('Edit the user name', async () => {
        await page.locator('input[name="FirstName"]').fill('Updated Browser');
        await page.getByRole('button', { name: /Save|Update/ }).click();
        await expect(page).toHaveURL(/\/data\/users$/);
    });
    await test.step('Verify the edit feedback and updated value', async () => {
        await expect(page.getByRole('alert')).toContainText('User updated');
        await expect(page.getByText('Updated Browser', { exact: true })).toBeVisible();
    });
    await test.step('Open the user delete confirmation', async () => {
        const row = page.locator('tr').filter({ hasText: username });
        await row.getByRole('link', { name: 'Delete' }).click();
        await expect(page.getByRole('heading', { name: /Delete User/ })).toBeVisible();
    });
    await test.step('Confirm deletion in the browser', async () => {
        await page.getByRole('button', { name: 'Delete' }).click();
        await expect(page).toHaveURL(/\/data\/users$/);
    });
    await test.step('Verify the user was deleted', async () => {
        await expect(page.getByRole('alert')).toContainText('User deleted');
        await expect(page.getByText(username, { exact: true })).not.toBeVisible();
    });
});
