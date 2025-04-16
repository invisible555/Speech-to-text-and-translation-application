import { useEffect, useState } from 'react';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css'
import Login from "./Components/Login/Login"
import Register from "./Components/Register/Register"
import LoginButton from './Components/LoginButton/LoginButton';
import RegisterButton from './Components/RegisterButton/RegisterButton';

function App() {
    const [forecasts, setForecasts] = useState();
    const [contents, setContents] = useState();
    useEffect(() => {
        populateWeatherData();

        
    }, []);

    const loginButtonHandler = () => {
      
        setContents(<Login />);

    };

    const registerButtonHandler = () => {

        setContents(<Register />);
        //  setContents(forecasts)

    };


    
    return (
        <div>
            <LoginButton onClick={loginButtonHandler} />
            <RegisterButton onClick={registerButtonHandler } />
            <h1 id="tableLabel">Weather forecast</h1>
            <p>This component demonstrates fetching data from the server.</p>
          
            {contents}
        </div>
    );
    
    async function populateWeatherData() {
        const response = await fetch('weatherforecast');
        if (response.ok) {
            const data = await response.json();
            setForecasts(data);
        }
    }
}

export default App;