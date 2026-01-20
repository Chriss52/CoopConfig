import { useReducer } from "react";

type DialogState = { id: number; visible: boolean };

type DialogAction =
  | { type: "CLOSE_DIALOG" }
  | { type: "OPEN_DIALOG"; payload: number };

const DialogReducer = (
  state: DialogState,
  action: DialogAction,
): DialogState => {
  if (action.type === "OPEN_DIALOG")
    return { ...state, visible: true, id: action.payload };
  if (action.type === "CLOSE_DIALOG")
    return { ...state, id: 0, visible: false };
  return state;
};

/**
 * Custom hook to manage dialog state using a reducer.
 * @param initialState - The initial state of the dialog (default: { id: 0, open: false }).
 * @returns A tuple containing the current dialog state and a dispatch function.
 */
const useDialogReducer = (
  initialState: DialogState = { id: 0, visible: false },
) => {
  const [state, dispatch] = useReducer(DialogReducer, initialState);
  return {
    state,
    close: () => dispatch({ type: "CLOSE_DIALOG" }),
    open: (id: number) => dispatch({ payload: id, type: "OPEN_DIALOG" }),
  };
};

export default useDialogReducer;
