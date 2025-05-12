import React from 'react';
import HomeElementType from "./HomeElementType";
import './HomeElement.css';

const HomeElement: React.FC<HomeElementType> = ({ title, description, image }) => {
    return (
        <div className="card home-card shadow-sm border-light">
            <img src={image} className="card-img-top" alt="Home Element" />
            <div className="card-body">
                <h5 className="card-title">{title}</h5>
                <p className="card-text">{description}</p>
            </div>
        </div>
    );
};

export default HomeElement;
