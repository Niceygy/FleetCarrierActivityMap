//x,y,z,opacity <- plot.csv
d3.csv("plot.csv", function (err, rows) {
  function unpack(rows, key) {
    return rows.map(function (row) {
      return row[key];
    });
  }

  var data = [
    {
      x: unpack(rows, "x"),
      y: unpack(rows, "y"),
      z: unpack(rows, "z"),
      mode: "markers",
      marker: {
        size: 12,
        color: rows.map((row) => `rgba(41,70,${row.opacity},0.5)`),
        line: {
          color: rows.map((row) => `rgba(41,70,${row.opacity},0.5)`),
          width: 1,
        },
      },
      text: unpack(rows, "label"), // Add this line to provide hover labels
      hoverinfo: "text", // Show only the label on hover
      type: "scatter3d",
    },
  ];
  var layout = {
    margin: {
      l: 0,
      r: 0,
      b: 0,
      t: 0,
    },
    autosize: true,
    width: window.innerWidth,
    height: window.innerHeight,
  };
  Plotly.newPlot("myDiv", data, layout, { responsive: true });
  window.addEventListener("resize", () => {
    Plotly.Plots.resize("myDiv");
  });
});
