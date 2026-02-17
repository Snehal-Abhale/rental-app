/* eslint-disable no-unused-vars */
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import axios from 'axios';

// 1. Fetch All
// 1. Fetch All (Supports Search)
export const fetchListings = createAsyncThunk('listings/fetchAll', async (searchTerm, { rejectWithValue }) => {
    try {
        let url = '/api/Listings';
        // If searchTerm is provided (and is a string), use the search endpoint
        if (searchTerm && typeof searchTerm === 'string') {
            url = `/api/Listings/search?q=${encodeURIComponent(searchTerm)}`;
        }
        const response = await axios.get(url);
        return response.data;
    } catch (error) {
        return rejectWithValue(error.response?.data || "Error fetching listings");
    }
});

// 2. Fetch Single (NEW)
export const fetchListingById = createAsyncThunk('listings/fetchById', async (id, { rejectWithValue }) => {
    try {
        const response = await axios.get(`/api/Listings/${id}`);
        return response.data;
    } catch (error) {
        return rejectWithValue(error.response.data);
    }
});

const listingsSlice = createSlice({
    name: 'listings',
    initialState: {
        items: [],
        currentListing: null, // <--- New State for Details page
        loading: false,
        error: null
    },
    reducers: {
        clearCurrentListing: (state) => {
            state.currentListing = null;
        }
    },
    extraReducers: (builder) => {
        builder
            // Fetch All
            .addCase(fetchListings.pending, (state) => { state.loading = true; })
            .addCase(fetchListings.fulfilled, (state, action) => {
                state.loading = false;
                state.items = action.payload;
            })
            // Fetch One
            .addCase(fetchListingById.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchListingById.fulfilled, (state, action) => {
                state.loading = false;
                state.currentListing = action.payload;
            })
            .addCase(fetchListingById.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload;
            });
    }
});

export const { clearCurrentListing } = listingsSlice.actions;
export default listingsSlice.reducer;