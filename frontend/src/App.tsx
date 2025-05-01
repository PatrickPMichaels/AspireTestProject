import { useEffect, useState } from "react";
import "./App.css";
import { Forecast } from "./models/Forecast";

function App() {
    const [forecasts, setForecasts] = useState<Array<Forecast>>([]);
    const [message, setMessage] = useState<string>();

    const requestWeather = async () => {
        const weather = await fetch("api/weatherforecast");
        console.log(weather);

        const weatherJson = await weather.json();
        console.log(weatherJson);

        setForecasts(weatherJson);
    };

    useEffect(() => {
        requestWeather();
    }, []);

    const saveForecast = () => {
        fetch("api/weatherforecast", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(forecasts)
        });
    }

    const sendMessage = () => {
        fetch("api/weatherforecast/message", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(message)
        });
    }

    return (
        <div className="App">
            <header className="App-header">
                <h1>React (Vite) Weather</h1>
                <table>
                    <thead>
                        <tr>
                            <th>Date</th>
                            <th>Temp. (C)</th>
                            <th>Temp. (F)</th>
                            <th>Summary</th>
                        </tr>
                    </thead>
                    <tbody>
                        {(
                            forecasts ?? [
                                {
                                    date: "N/A",
                                    temperatureC: "",
                                    temperatureF: "",
                                    summary: "No forecasts",
                                },
                            ]
                        ).map((w) => {
                            return (
                                <tr key={w.date}>
                                    <td>{w.date}</td>
                                    <td>{w.temperatureC}</td>
                                    <td>{w.temperatureF}</td>
                                    <td>{w.summary}</td>
                                </tr>
                            );
                        })}
                    </tbody>
                </table>
                <button onClick={saveForecast}>
                    Save Forecast
                </button>

                <div style={{marginTop:"2em"}}>
                    <label style={{marginRight:"1em"}} htmlFor="message">Message:</label>
                    <input style={{ marginRight: "1em" }} type="text" id="name" name="message" value={message} onChange={(e:React.ChangeEvent<HTMLInputElement>)=>setMessage(e.target.value)} />
                    <button onClick={sendMessage}>
                        Send Message
                    </button>
                </div>
            </header>
        </div>
    );
}

export default App;