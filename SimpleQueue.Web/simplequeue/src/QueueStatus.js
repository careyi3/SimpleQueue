import React, { Component } from "react";
import { Grid, Row, Col } from "./Layouts";
import QueueStatusLabel from "./QueueStatusLabel";

class QueueStatus extends Component {
  constructor(props) {
    super(props);
    this.state = {
      queueStatus: props.queueStatus,
      refreshPeriod: props.refreshPeriod,
    };
  }

  componentDidUpdate(previousProps) {
    if (this.props.queueStatus !== previousProps.queueStatus) {
      this.setState({ queueStatus: this.props.queueStatus });
    }
  }

  render() {
    const { queueStatus, refreshPeriod } = this.state;
    if (queueStatus) {
      return (
        <div>
          <h2>Status</h2>
          <p>The queue status will refresh every {refreshPeriod} seconds.</p>
          <Grid>
            <Row>
              {Object.keys(queueStatus).map((value) => {
                return (
                  <Col size={1}>
                    <QueueStatusLabel
                      label={value}
                      value={queueStatus[value]}
                    />
                  </Col>
                );
              })}
            </Row>
          </Grid>
        </div>
      );
    } else {
      return (
        <div>
          <p>Please select a queue to view status.</p>
        </div>
      );
    }
  }
}

export default QueueStatus;
