/// <reference types="Cypress" />

context('Actions', () => {
  beforeEach(() => {
    cy.visit('/');
  })

  it('checks that a new element is persisted.', () => {
    const todoText = 'Test the app';
    cy.get('input#new-todo')
      .type('Test the app').should('have.value', todoText)
      .get('#add-btn').click()
      .get('#todo-list').contains(todoText)

      // reload the page and verify that it is still there.
      .reload()
      .get('#todo-list').contains(todoText)
  });

  it('checks that a deleted element is removed.', () => {
    const todoText = 'Remove an item';
    cy.get('input#new-todo')
      .type(todoText).should('have.value', todoText)
      .get('#add-btn').click()
      .get('#todo-list').contains(todoText)
      .get('[data-cy=delete-btn]').click()
      .get('#todo-list').should('not.contain', todoText)

      // reload the page and verify that it is still gone.
      .reload()
      .get('#todo-list').should('not.contain', todoText)
  });
})
