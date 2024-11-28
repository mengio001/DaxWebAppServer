describe("My achievements page", () => {

  const selectors = {
    shareButton: 'sharethis-inline-button',
    copyLink: 'st-network-copy',
    facebook: 'st-network-facebook',
    twitter: 'st-network-twitter',
    linkedin: 'st-network-linkedin',
  };

  beforeEach(() => {
    cy.loginWithPKCE().then(() => {
      cy.visit('https://localhost:44365/my-results');
    });
  });

  // afterEach(() => {
  //   cy.clearCookies().then(() => {
  //     cy.clearLocalStorage();
  //   });
  // });

  context("PKCE Login Flow", () => {
    it("logs in and fetches user-related data", () => {
      cy.intercept('GET', '/account/user*', (req) => {
        req.on('response', (res) => {
          expect(res.statusCode).to.eq(200);
        });
      }).as('getUserSession');
    });
  })

  context("ShareThis (st) inline share button section", () => {

    it('should have the same number of rows and share buttons', () => {
      cy.intercept('GET', '/api/UserResult/MyResults/Sony', (req) => {
        req.on('response', (res) => {
          expect(res.statusCode).to.eq(200);
        });
      }).as('getUserResult');

      cy.wait('@getUserResult').then(() => {
        cy.getBySel('achievement-results').then(($rows) => {
          Cypress.env('rowCount', $rows.length);
          cy.wrap({ rowLength: $rows.length }).should('have.property', 'rowLength').as('asRowCount');
        });

        cy.get('@asRowCount').then(($asRowCount) => {
          expect($asRowCount).to.be.greaterThan(0);
          cy.getBySel(selectors.shareButton).should('have.length', $asRowCount);
        });
      });
    });

    it("should display the share menu when the Share button is clicked", () => {
      expect(Cypress.env('rowCount')).to.be.greaterThan(0);

      // Math.random() * 2 => generates a random number between 0 or 1
      const randomIndex: number = Math.floor(Math.random() * (Number(Cypress.env('rowCount'))));
      cy.getBySel(selectors.shareButton)
        .eq(randomIndex)
        .should("exist")
        .should("be.visible")
        .click();

      cy.getBySel(selectors.copyLink).contains("Copy Link").should("exist").should("be.visible");
      cy.getBySel(selectors.twitter).contains("Twitter").should("exist").should("be.visible");
      cy.getBySel(selectors.facebook).contains("Facebook").should("exist").should("be.visible");
      cy.getBySel(selectors.linkedin).contains("LinkedIn").should("exist").should("be.visible");
    });

  });

  context("Network Sharing Flows", () => {
    beforeEach(() => {
      cy.getBySel(selectors.shareButton).first().click();
      cy.getBySel(selectors.copyLink).contains("Copy Link").should("exist").should("be.visible");
    });

    it("should copy the link to the clipboard when 'Copy Link' is clicked", () => {
      // Mock clipboard to always succeed
      cy.window().then((win) => {
        cy.stub(win.navigator.clipboard, 'writeText').resolves();
      });

      cy.getBySel(selectors.copyLink).contains("Copy Link").click();

      cy.get('mat-dialog-content')
        .should("be.visible")
        .should("contain.text", "Link copied to clipboard!");

      cy.get('button[mat-dialog-close]').should("be.visible").click();
      cy.get('mat-dialog-content').should("not.exist");
    });

    it("should open the Facebook sharing popup when the 'Facebook' button is clicked", () => {
      // Spy on window.open
      cy.window().then((win) => {
        cy.stub(win, 'open').as('windowOpen');
      });

      // Simulate clicking the Twitter share button
      cy.getBySel(selectors.facebook).click();

      // Assert that window.open was called with the correct URL
      cy.get('@windowOpen').should('be.calledWithMatch', /https:\/\/www.facebook.com\/sharer\/sharer.php\?u=/);

      // Assert that the fallback dialog is visible
      cy.get('mat-dialog-content')
        .should('be.visible')
        .and('contain.text', 'Your achievement is climbing the Tower of Quizzes');

      cy.get('button[mat-dialog-close]').should("be.visible").and('contain.text', 'Cancel').click();
      cy.get('mat-dialog-content').should("not.exist");
    });

    it('should open the Twitter sharing popup when the "Twitter" button is clicked', () => {
      cy.window().then((win) => {
        cy.stub(win, 'open').as('windowOpen');
      });

      cy.getBySel(selectors.twitter).click();

      cy.get('@windowOpen').should('be.calledWithMatch', /https:\/\/x.com\/intent\/post\?url=/);

      cy.get('mat-dialog-content')
        .should('be.visible')
        .and('contain.text', 'Your achievement is climbing the Tower of Quizzes');

      cy.get('button[mat-dialog-close]').should("be.visible").and('contain.text', 'Cancel').click();
      cy.get('mat-dialog-content').should("not.exist");
    });

    it("should open the LinkedIn sharing popup when the 'LinkedIn' button is clicked", () => {
      cy.window().then((win) => {
        cy.stub(win, 'open').as('windowOpen');
      });

      cy.getBySel(selectors.linkedin).click();

      cy.get('@windowOpen').should('be.calledWithMatch', /https:\/\/www.linkedin.com\/sharing\/share\-offsite\/\?url=/);
 
      cy.get('mat-dialog-content')
        .should('be.visible')
        .and('contain.text', 'Your achievement is climbing the Tower of Quizzes');

      cy.get('button[mat-dialog-close]').should("be.visible").and('contain.text', 'Cancel').click();
      cy.get('mat-dialog-content').should("not.exist");
    });

  });

});