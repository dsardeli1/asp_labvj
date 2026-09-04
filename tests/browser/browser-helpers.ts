import { expect, type Page } from '@playwright/test';

export async function signIn(page: Page): Promise<void> {
    await page.goto('/account/login');
    await page.getByLabel('Username or Email').fill('ana.kovacic');
    await page.getByLabel('Password').fill('Password123!');
    await page.getByRole('button', { name: 'Sign in' }).click();
    await expect(page).not.toHaveURL(/\/account\/login/);
}

export async function chooseAutocomplete(page: Page, fieldId: string, searchText: string): Promise<void> {
    const input = page.locator(`#${fieldId}`);
    await input.fill(searchText);
    const menu = page.locator(`#${fieldId}__listbox`);
    await expect(menu).toBeVisible();
    await menu.getByRole('option').first().click();
}
