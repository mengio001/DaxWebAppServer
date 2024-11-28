/// <reference types="cypress" />
/// <reference path="./commands.d.ts" />

Cypress.Commands.add("getBySel", (selector, ...args) => {
    return cy.get(`[data-test=${selector}]`, ...args);
});

Cypress.Commands.add("getBySelLike", (selector, ...args) => {
    return cy.get(`[data-test*=${selector}]`, ...args);
});

Cypress.Commands.add(
    "loginWithPKCE",
    (username: string | null = null, password: string | null = null) => {
        const authorizeUrl = `${Cypress.env("baseUrl")}/account/login`;
        const fb_name = Cypress.env("username") || "";
        const fb_password = Cypress.env("password") || "";
        cy.visit(authorizeUrl).then(() => {
            cy.get('input[name="Input.Username"]').should("be.visible").type(username ?? fb_name);
            cy.get('input[name="Input.Password"]').type(password ?? fb_password);
            cy.get('button[name="Input.Button"].btn-primary').click();
        });
    }
);