describe('home page', () => {
  beforeEach(() => {
    cy.clearCookies();
    cy.clearLocalStorage();

    cy.intercept('GET', '/account/user*', (req) => {
      req.on('response', (res) => {
        expect(res.statusCode).to.eq(401);
        if (res.statusCode === 401) {
          res.body = JSON.stringify({});
          res.headers['content-type'] = 'application/json'; 
          res.statusCode = 401;
        }
      });
    }).as('getUserSession');

    cy.visit("https://localhost:44365/home")
    cy.get("#app-root").should("exist").should("be.visible");
    cy.get('[data-cy="router-outlet"]').should("be.empty");
    cy.get('app-home').should("be.visible");
  })

  it("the h1 and sub-title contains the correct text", () => {
    cy.get('[data-test="welcome-title"]').contains("Tower of Quizzes")
    cy.get('[data-test="welcome-description"]').contains("Please log in to see your Quiz items.")    
  })

  it('should have coverage instrumentation', () => {
    cy.window().then((win) => {
      expect(win.__coverage__).to.exist;
    });
  });
})