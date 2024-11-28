/// <reference types="cypress" />

declare namespace Cypress {
    interface Chainable {
        /**
         * Custom command to get an element by data-test attribute.
         * @param dataTestAttribute - The data-test attribute of the element
         */
        getByData(dataTestAttribute: string): Chainable<JQuery<HTMLElement>>
        getBySel(dataTestAttribute: string, args?: any): Chainable<JQuery<HTMLElement>>;
        getBySelLike(dataTestPrefixAttribute: string, args?: any): Chainable<JQuery<HTMLElement>>;

        /**
         * Custom command to get an access token from IdentityServer.
         * @param client_id - The client_id for machine-to-machine testing
         * @param client_secret - The client_secret for machine-to-machine testing
         */
        getAccessTokenIdp(client_id: string | null = null, client_secret: string | null = null): Chainable<string>;

        /**
         * Custom command to get an access token from IdentityServer.
         * @param username - The username for login
         * @param password - The password for login
         */
        loginWithPKCE(username: string | null = null, password: string | null = null): Chainable<string>;
    }

    interface TestConfigOverrides {
        rowCount: number;
    }
}