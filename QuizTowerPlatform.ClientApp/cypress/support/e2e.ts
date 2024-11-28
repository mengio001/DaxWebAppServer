/// <reference types="cypress" />

// Import commands.js using ES2015 syntax:
import './commands.ts'

// Alternatively you can use CommonJS syntax:
// require('./commands')

import '@cypress/code-coverage/support';

Cypress.on('uncaught:exception', (err) => {
    if ((err.message && err.message.includes('postMessage')) || (err.stack && err.stack.includes('PrimaryOriginCommunicator.toSource'))) {
        return false;
    }
    return true;
});

Cypress.on('window:before:load', (win) => {
    win.addEventListener('unhandledrejection', (event) => {
        if (event.reason?.message?.includes('postMessage') || event.reason?.stack?.includes('PrimaryOriginCommunicator.toSource')) {
            event.preventDefault();
        }
    });
});
