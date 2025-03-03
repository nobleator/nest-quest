document.addEventListener("DOMContentLoaded", async () => {
    const response = await fetch("http://localhost:5000/api/v0/hello");
    const data = await response.text();
    document.getElementById("output").innerText = data;
  });
document.addEventListener("DOMContentLoaded", async () => {
  const response = await fetch("http://localhost:5000/api/v0/hello");
  const data = await response.text();
  document.getElementById("output").innerText = data;
});
  