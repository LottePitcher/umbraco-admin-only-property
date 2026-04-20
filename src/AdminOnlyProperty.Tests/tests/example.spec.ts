import { expect, test } from '@playwright/test';

test.describe('Check user group access', () => {

  test.beforeEach(async ({ page }, testInfo) => {
    await page.goto(process.env.URL + '/umbraco');
  });

  test('Can see property as admin', async ({page}) => {
    let error = page.locator('.text-error');
    await expect(error).toBeHidden();

    // Action
    await page.fill('#umb-username', process.env.UMBRACO_ADMIN_USER_LOGIN);
    await page.fill('#umb-passwordTwo', process.env.UMBRACO_USER_PASSWORD);
    await page.locator('[label-key="general_login"]').click();
    await page.waitForNavigation();

    // Assert
    await expect(page).toHaveURL(process.env.URL + '/umbraco#/content');
    let usernameField = await page.locator('#umb-username');
    let passwordField = await page.locator('#umb-passwordTwo');
    await expect(usernameField).toHaveCount(0);
    await expect(passwordField).toHaveCount(0);

    // Navigate to Restricted and Other Properties node
    await page.getByRole('link', { name: 'Restricted and Other Properties' }).click();

    // Expect the Indicator property to be visible
    await expect(page.getByText('Textstring (All)')).toBeVisible();
    await expect(page.getByText('Indicator')).toBeVisible();
  });

  test('Cannot see indicator property as editor', async ({page}) => {
    let error = page.locator('.text-error');
    await expect(error).toBeHidden();

    // Action
    await page.fill('#umb-username', process.env.UMBRACO_EDITOR_USER_LOGIN);
    await page.fill('#umb-passwordTwo', process.env.UMBRACO_USER_PASSWORD);
    await page.locator('[label-key="general_login"]').click();
    await page.waitForNavigation();

    // Assert
    await expect(page).toHaveURL(process.env.URL + '/umbraco#/content');
    let usernameField = await page.locator('#umb-username');
    let passwordField = await page.locator('#umb-passwordTwo');
    await expect(usernameField).toHaveCount(0);
    await expect(passwordField).toHaveCount(0);

    // Navigate to Restricted and Other Properties node
    await page.getByRole('link', { name: 'Restricted and Other Properties' }).click();

    // Expect the Indicator property to be visible
    await expect(page.getByText('Textstring (All)')).toBeVisible();
    await expect(page.getByText('Indicator')).toBeHidden();
  });

});


