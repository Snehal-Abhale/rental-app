import { configureStore } from '@reduxjs/toolkit';
import authReducer from './slices/authslice';
import listingsReducer from './slices/listingsSlice';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    listings: listingsReducer // <--- Add this line
  },
});