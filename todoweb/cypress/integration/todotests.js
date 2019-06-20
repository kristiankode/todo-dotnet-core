/// <reference types="Cypress" />

context('Actions', () => {
  beforeEach(() => {
    cy.visit('/');
  })

  it('checks that a new element is persisted.', () => {
    const todoText = 'Add an item';
    cy.get('input#new-todo')
      .type(todoText).should('have.value', todoText)
      .get('#add-btn').click()
      .get('#todo-list').contains(todoText)

      // reload the page and verify that it is still there.
      .reload()
      .get('#todo-list').contains(todoText)
  });

  it('checks that a deleted item does not reappear', () => {

    cy.get('input#new-todo')
      .type('Delete me plz')
      .should('have.value', 'Delete me plz')
      .get('#add-btn').click()
      .reload()

      .get('#todo-list')
      .contains('Delete me plz').find('[data-cy=delete-btn]').click()
      .get('#todo-list').should('not.contain', 'Delete me plz')

      .reload()
      .get('#todo-list')
      .should('be.visible')
      .should('not.contain', 'Delete me plz')
  });

})
