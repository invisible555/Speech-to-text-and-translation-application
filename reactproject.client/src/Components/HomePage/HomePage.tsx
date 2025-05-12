import React from 'react';
import HomeElement from "../HomeElement/HomeElement";

const homeData = [
    {
        title: "Szybkość działania",
        description: "Nasza aplikacja działa błyskawicznie na każdej przeglądarce.",
        image: "/speed.png"
    },
    {
        title: "Bezpieczeństwo",
        description: "Twoje dane są u nas bezpieczne - korzystamy z nowoczesnych zabezpieczeń.",
        image: "/security.png"
    },
    {
        title: "Intuicyjność",
        description: "Prosty i intuicyjny interfejs użytkownika.",
        image: "/intuition.png"
    },
    {
        title: "Wsparcie",
        description: "Szybkie wsparcie techniczne dla każdego użytkownika.",
        image: "/help.png"
    }
];

const HomePage: React.FC = () => {
    return (
        <div className="container mt-5">
            <h1 className="text-center mb-4">Nasze Atuty</h1>
            <div className="d-flex flex-wrap justify-content-center">
                {homeData.map((item, index) => (
                    <HomeElement
                        key={index}
                        title={item.title}
                        description={item.description}
                        image={item.image}
                    />
                ))}
            </div>
        </div>
    );
};

export default HomePage;
