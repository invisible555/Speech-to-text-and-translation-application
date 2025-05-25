import { useEffect, useState } from "react";
import { Outlet } from 'react-router-dom';
import axiosInstance from "../../../utils/axiosConfig";

const Profile = () => {
    const [userLogin,setUserLogin] = useState<string | null>(null);
    const [userEmail,setUserEmail] = useState<string | null>(null);
    const [userRole,setUserRole] = useState<string | null>(null);
    useEffect( () => {

        const getProfile = async () =>
        {
        try{
        const response = await axiosInstance.get("User/profile");
        const data = response.data;
        setUserLogin(data.login);
        setUserEmail(data.email)
        setUserRole(data.role)


        }
        catch (err:any)
        {
             console.error('Błąd przy pobieraniu profilu:', err);
            setUserLogin("Błąd")

        }
    }
    getProfile()
},[])
        
return ( <div>
      <h2>Profil użytkownika</h2>
      <p>Login: {userLogin}</p>
      <p>Email: {userEmail}</p>
      <p>Rola: {userRole}</p>
      <Outlet/>
    </div>
    )
}

export default Profile