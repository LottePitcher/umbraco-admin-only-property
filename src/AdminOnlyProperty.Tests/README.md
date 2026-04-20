# Playwright tests

## Setup

To get started run:

```
npm install
```

Then ensure the website is running and has both an admin and editor user.

Finally ensure the url of the site and the login usernames/passwords of the users correspond to the values in the .env file in this folder.

Finally to run the testsuite use:

```
npx playwright test
```

To run it in a window instead of the console use:

```
npx playwright test --ui
```