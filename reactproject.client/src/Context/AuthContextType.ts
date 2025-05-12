interface AuthContextType {
    isLoggedIn: boolean;
    userLogin: string | null;
    token: string | null;
    role: string | null;
    login: (token: string,user:string) => void;
    logout: () => void;
    navigateToLogin: () => void,
}

export default AuthContextType