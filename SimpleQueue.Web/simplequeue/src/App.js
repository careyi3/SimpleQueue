import React, { Component } from "react";
import QueueStatus from "./QueueStatus";
import "./App.css";
import Select from "react-select";

class App extends Component {
  constructor(props) {
    super(props);
    this.state = {
      error: null,
      isLoaded: false,
      selectOptions: [],
      selectedQueue: null,
      queueStatus: null,
      pollingDelay: 500,
    };

    this.handleQueueSelectChange = this.handleQueueSelectChange.bind(this);
  }

  handleQueueSelectChange(event) {
    clearInterval(this.interval);
    let state = {
      selectedQueue: null,
      queueStatus: null,
    };
    if (event && event.value) {
      state.selectedQueue = event.value;
    }
    this.setState(state);
  }

  componentDidUpdate() {
    if (this.state.selectedQueue) {
      clearInterval(this.interval);
      this.interval = setInterval(this.poll, this.state.pollingDelay);
    }
  }

  componentDidMount() {
    this.interval = setInterval(this.poll, this.state.pollingDelay);
    fetch("http://localhost:5000/queue")
      .then((res) => res.json())
      .then(
        (result) => {
          this.setState({
            isLoaded: true,
            selectOptions: result.map((x) => {
              return { value: x.id, label: x.name };
            }),
          });
        },
        (error) => {
          this.setState({
            isLoaded: true,
            error,
          });
        }
      );
  }

  componentWillUnmount() {
    clearInterval(this.interval);
  }

  poll = () => {
    if (this.state.selectedQueue) {
      fetch(`http://localhost:5000/queue/${this.state.selectedQueue}`)
        .then((res) => res.json())
        .then(
          (result) => {
            this.setState({
              queueStatus: result,
            });
          },
          (error) => {
            this.setState({
              isLoaded: true,
              error,
            });
            clearInterval(this.interval);
          }
        );
    }
  };

  render() {
    const {
      error,
      isLoaded,
      selectOptions,
      queueStatus,
      pollingDelay,
    } = this.state;
    if (error) {
      return <div>Error: {error.message}</div>;
    } else if (!isLoaded) {
      return <div>Loading...</div>;
    } else {
      return (
        <div className="body">
          <div>
            <h2>Queues</h2>
            <Select
              options={selectOptions}
              onChange={this.handleQueueSelectChange}
              isClearable={true}
            />
          </div>
          <QueueStatus
            queueStatus={queueStatus}
            refreshPeriod={pollingDelay / 1000}
          />
        </div>
      );
    }
  }
}

export default App;
