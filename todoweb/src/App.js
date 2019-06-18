import React, { Component } from 'react'
/*
Todo app structure

TodoApp
	- TodoHeader
	- TodoList
    - TodoListItem #1
		- TodoListItem #2
		  ...
		- TodoListItem #N
	- TodoForm
*/
var todoItems = [];

class TodoList extends Component {
  render () {
    var items = this.props.items.map((item, index) => {
      return (
        <TodoListItem key={index} item={item} index={index} removeItem={this.props.removeItem} markTodoDone={this.props.markTodoDone} />
      );
    });
    return (
      <ul className="list-group" id="todo-list"> {items} </ul>
    );
  }
}
  
class TodoListItem extends Component {
  constructor(props) {
    super(props);
    this.onClickClose = this.onClickClose.bind(this);
    this.onClickDone = this.onClickDone.bind(this);
  }
  onClickClose() {
    var index = parseInt(this.props.index);
    this.props.removeItem(index);
  }
  onClickDone() {
    var index = parseInt(this.props.index);
    this.props.markTodoDone(index);
  }
  render () {
    var todoClass = this.props.item.done ? 
        "done" : "undone";
    return(
      <li className="list-group-item ">
        <div className={todoClass}>
          <span className="glyphicon glyphicon-ok icon" aria-hidden="true" onClick={this.onClickDone}></span>
          {this.props.item.value}
          <button type="button" className="close" 
            data-cy="delete-btn"
            onClick={this.onClickClose}>&times;</button>
        </div>
      </li>     
    );
  }
}

class TodoForm extends Component {
  constructor(props) {
    super(props);
    this.onSubmit = this.onSubmit.bind(this);
  }
  componentDidMount() {
    this.refs.itemName.focus();
  }
  onSubmit(event) {
    event.preventDefault();
    var newItemValue = this.refs.itemName.value;
    
    if(newItemValue) {
      this.props.addItem({newItemValue});
      this.refs.form.reset();
    }
  }
  render () {
    return (
      <form ref="form" onSubmit={this.onSubmit} className="form-inline">
        <input id="new-todo"
        type="text" ref="itemName" className="form-control" placeholder="add a new todo..."/>
        <button id="add-btn" type="submit" className="btn btn-default">Add</button> 
      </form>
    );   
  }
}

const TodoHeader = props => <h1>{props.name}</h1>
  
class TodoApp extends Component {
  state = {
    listId: null,
    listName: null,
    todoItems: [],
    loading: true
  }
  constructor (props) {
    super(props);
    this.addItem = this.addItem.bind(this);
    this.removeItem = this.removeItem.bind(this);
    this.markTodoDone = this.markTodoDone.bind(this);
  }

  componentWillMount(){
    fetch('http://localhost:60154/todos')
    .then(results => results.json())
    .then(lists => lists && lists.length ? lists[0] : {index: 0, name: "No todolist found", todoItems: []})
    .then(list => {
      console.log(list);
      var todos = list.todoItems.map(todo => (
        {
          index: todo.id, 
          value: todo.name,
          done: todo.isComplete
        }));

      this.setState({
        listId: list.id,
        listName: list.name,
        todoItems: todos,
        loading: false
      });
    })
  }
  addItem(todoItem) {
    todoItems.unshift({
      index: todoItems.length+1, 
      value: todoItem.newItemValue, 
      done: false
    });
    this.setState({todoItems: todoItems});
  }
  removeItem (itemIndex) {
    todoItems.splice(itemIndex, 1);
    this.setState({todoItems: todoItems});
  }
  markTodoDone(itemIndex) {
    var todo = todoItems[itemIndex];
    todoItems.splice(itemIndex, 1);
    todo.done = !todo.done;
    todo.done ? todoItems.push(todo) : todoItems.unshift(todo);
    this.setState({todoItems: todoItems});  
  }
  render() {
    if(this.state.loading){
      return(
      <div id="main">
        <TodoHeader name="Loading..." />
      </div>
      );
    }
    return (
      <div id="main">
        <TodoHeader name={this.state.listName}/>
        <TodoList items={this.state.todoItems} removeItem={this.removeItem} markTodoDone={this.markTodoDone}/>
        <TodoForm addItem={this.addItem} />
      </div>
    );
  }
}

export default TodoApp
