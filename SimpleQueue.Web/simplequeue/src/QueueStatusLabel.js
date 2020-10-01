import React, { Component } from "react";
import { Grid, Row, Col } from "./Layouts";
import "./App.css";

class QueueStatusLabel extends Component {
  constructor(props) {
    super(props);
    this.state = {
      label: props.label,
      value: props.value,
    };
  }

  componentDidUpdate(previousProps) {
    if (this.props.value !== previousProps.value) {
      this.setState({ value: this.props.value });
    }
  }

  render() {
    const { label, value } = this.state;
    return (
      <Grid>
        <Col size={1}>
          <Row>
            <h2>{label}</h2>
          </Row>
          <Row>
            <p className="label-xl">{value}</p>
          </Row>
        </Col>
      </Grid>
    );
  }
}

export default QueueStatusLabel;
