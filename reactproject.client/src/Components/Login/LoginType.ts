interface LoginType {
    onSubmit: (data: { login: string; password: string }) => void;
}

export default LoginType