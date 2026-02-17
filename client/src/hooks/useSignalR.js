
import { useEffect } from 'react';
import * as signalR from '@microsoft/signalr';
import { toast } from 'react-toastify';

export default function useSignalR() {
    useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub")
            .withAutomaticReconnect()
            .build();

        connection.start()
            .then(() => console.log('SignalR Connected!'))
            .catch(err => console.error('SignalR Connection Error: ', err));

        connection.on("ReceiveNotification", (message) => {
            toast.info(message, {
                position: "top-right",
                autoClose: 5000,
            });
        });

        // Clean up
        return () => {
            connection.off("ReceiveNotification");
            connection.stop();
        };
    }, []);
}