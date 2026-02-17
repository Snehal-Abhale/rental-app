import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { fetchListings } from '../store/slices/listingsSlice';

export default function SearchBar() {
    const [searchTerm, setSearchTerm] = useState('');
    const dispatch = useDispatch();

    const handleSearch = (e) => {
        e.preventDefault();
        dispatch(fetchListings(searchTerm));
    };

    const handleClear = () => {
        setSearchTerm('');
        dispatch(fetchListings());
    };

    return (
        <form onSubmit={handleSearch} className="d-flex mb-4 gap-2">
            <input
                type="text"
                className="form-control"
                placeholder="Search by location or name..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
            />
            <button className="btn btn-primary" type="submit">Search</button>
            {searchTerm && (
                <button className="btn btn-outline-secondary" type="button" onClick={handleClear}>Clear</button>
            )}
        </form>
    );
}
