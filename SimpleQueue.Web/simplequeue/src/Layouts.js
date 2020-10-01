import styled from "styled-components";

export const Grid = styled.div``;

export const Row = styled.div`
  display: flex;
  align-items: top;
  justify-content: center;
`;

export const Col = styled.div`
  flex: ${(props) => props.size};
`;
