import logo from "./logo.svg";
import "./App.css";
import { useEffect, useState } from "react";
import axios from "axios";

function App() {
  const [data, setData] = useState(null);
  const [error, setError] = useState(null);
  useEffect(() => {
    console.log("started app");
    async function getToken() {
      // todo;
      // call http://localhost:5726/Home/Token
      let token = "";
      try {
        token = await axios.get("http://localhost:5726/Home/Token", {
          withCredentials: true,
        });
      } catch (e) {
        // error
        window.location.href = "http://localhost:5726";
      }
      // if 200 ok=> store the returned token in local storage
      // then call http://localhost:5000
      try {
        const config = await axios.get("http://localhost:5000/configs", {
          headers: {
            Authorization: `Bearer ${token.data}`,
          },
        });
        console.log(config.data);
        setData(config.data);
      } catch (error) {
        setError(error || "Error loading form configs");
      }
    }
    getToken();
  }, []);

  return (
    <div className="App">
      {error && <div>{error}</div>}
      {data && data.map((x) => (
        <div>
          <h1>{x.section}</h1>
          <div>
            <ol>
              {x.questions.map((q) => (
                <div>
                  {q.label}: <input type="text"></input>
                </div>
              ))}
            </ol>
          </div>
        </div>
      ))}
    </div>
  );
}

export default App;
